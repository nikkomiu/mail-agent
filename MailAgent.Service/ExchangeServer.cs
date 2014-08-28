using MailAgent.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.Exchange.WebServices.Data;
using System.Text.RegularExpressions;

namespace MailAgent.Service
{
    public class ExchangeServer
    {
        private ExchangeService _exService;

        private FolderId _errorFolderId;
        private string _errorFolderName;

        private FolderId _successFolderId;
        private string _successFolderName;

        private string _exportFilename;
        private string _globalSavePath;

        private Logging _logger;

        private string _emailAddress;

        public ExchangeServer(Dictionary<string, string> settings, Logging logger) : this(settings)
        {
            this._logger = logger;
        }

        public ExchangeServer(Dictionary<string, string> settings)
        {
            // Set the version of Exchange from settings file
            this._exService = new ExchangeService(GetVersionFromString(settings["MailExchangeVersion"]));

            // If not using windows credentials (from settings file) use manually define credentials
            if (settings["MailUseWindowsCredentials"].ToUpper() == "TRUE")
            {
                // Use Windows Credentials
                _exService.UseDefaultCredentials = true;
            }
            else
            {
                // Do not use Windows Credentials
                _exService.UseDefaultCredentials = false;

                // Set the login credentials
                _exService.Credentials = new NetworkCredential(settings["MailEmail"], settings["MailPassword"]);
            }

            _emailAddress = settings["MailEmail"];

            // If URL is Auto
            if (settings["MailUrl"].ToUpper() == "AUTO")
            {
                // Autodiscover based on email address
                _exService.AutodiscoverUrl(settings["MailEmail"]);
            }
            else
            {
                // Set the URL manually
                _exService.Url = new Uri(settings["MailUrl"]);
            }

            // Set the error folder name from the settings
            _errorFolderName = settings["MailErrorFolder"];
            _successFolderName = settings["MailSuccessFolder"];
            _exportFilename = settings["ExportFilename"];
            _globalSavePath = settings["DefaultSavePath"];
        }

        public void SaveMail(List<Profile> Profiles, Logging log)
        {
            // Get all of the mailbox items
            FindItemsResults<Item> inboxItems = _exService.FindItems(WellKnownFolderName.Inbox, new ItemView(100));

            // List of mailbox items that have been processed
            List<Item> completeInboxItems = new List<Item>();

            foreach (Profile profile in Profiles)
            {
                // Create a new temporary string for writing the output
                string localExportText = string.Empty;

                // Loop through all of the mail items that are in the inbox
                foreach (Item mailItem in inboxItems)
                {
                    try
                    {
                        // Log the subject of the email being processed
                        log.WriteLine(Logging.Level.DEBUG, "Current Item (Subject): " + mailItem.Subject);

                        // Load the item to get the body + attachments
                        mailItem.Load();
                        
                        // If the profile has an alias set and the email is not to the alias address
                        // skip the current email
                        string toAddress = GetToAddress(mailItem);

                        if (!string.IsNullOrEmpty(profile.Alias))
                        {
                            if (toAddress != null && !profile.Alias.Contains(toAddress))
                            {
                                continue;
                            }
                        }
                        else if (string.IsNullOrEmpty(toAddress) || !toAddress.Contains(_emailAddress))
                        {
                            continue;
                        }

                        // If the profile has an email subject to match then check the email subject for the string
                        // and if the email subject does not contain the profile subject skip the current email
                        if (!string.IsNullOrEmpty(profile.EmailSubject) && !mailItem.Subject.Contains(profile.EmailSubject))
                        {
                            continue;
                        }

                        // If the profile has an email body to match then check the email body for the string
                        // and if the email body does not contain the profile body skip the current email
                        if (profile.EmailBody.Length > 0 && !mailItem.Body.Text.Contains(profile.EmailBody))
                        {
                            continue;
                        }
                        
                        // Save the body if the profile wants it saved
                        if (profile.SaveEmailBody)
                        {
                            localExportText += WriteEmailBodyForProfile(mailItem, profile);
                        }

                        // Save the attachments if the profile wants them saved
                        if (mailItem.HasAttachments && profile.SaveAttachments)
                        {
                            localExportText += WriteAttachmentsForProfile(mailItem, profile);
                        }

                        // Move the email to the success folder
                        MoveItemToFolder(mailItem, _successFolderName, _successFolderId);
                        log.WriteLine(Logging.Level.INFO, "Moved " + mailItem.Subject + " to " + _successFolderName + " folder");
                    }
                    catch (Exception ex)
                    {
                        // Log the error
                        log.WriteError(ex);

                        if (ex.GetType() == new System.UnauthorizedAccessException().GetType())
                        {
                            log.WriteLine(Logging.Level.CRITICAL, "------------ Friendly Error ------------");
                            log.WriteLine(Logging.Level.CRITICAL, "  The service is probably being ran as");
                            log.WriteLine(Logging.Level.CRITICAL, "  the wrong user, make sure the user");
                            log.WriteLine(Logging.Level.CRITICAL, "  can access the path provided");
                            log.WriteLine(Logging.Level.CRITICAL, "----------------------------------------");
                        }

                        // Move the email to the error folder
                        MoveItemToFolder(mailItem, _errorFolderName, _errorFolderId);
                    }

                    // Add the item to the complete list
                    completeInboxItems.Add(mailItem);

                    // Sleep so there is time between emails
                    System.Threading.Thread.Sleep(5);
                }

                // Write the export file if there were emails processed and completed
                if (completeInboxItems.Count > 0 && localExportText.Length > 0)
                {
                    // Write the export file to the profile's save path
                    File.WriteAllText(profile.SavePath + DateTime.Now.ToFileTime() + "_" + _exportFilename, localExportText);
                }
            }

            // Move the items still in the inbox to the error folder
            foreach (Item mailItem in inboxItems)
            {
                // If the item has already been processed skip it
                if (completeInboxItems.Contains(mailItem))
                {
                    continue;
                }

                // Move the email to the error folder
                MoveItemToFolder(mailItem, _errorFolderName, _errorFolderId);
            }
        }

        private string GetToAddress(Item mailItem)
        {
            ExtendedPropertyDefinition propertyDefinition = new ExtendedPropertyDefinition(0x007D, MapiPropertyType.String);
            PropertySet propertySet = new PropertySet(BasePropertySet.FirstClassProperties) { propertyDefinition, ItemSchema.MimeContent };

            mailItem.Load(propertySet);
            Object headerValues;
            if (mailItem.TryGetProperty(propertyDefinition, out headerValues))
            {
                Regex regex = new Regex("To:.*<(.*)>");
                Match match = regex.Match(headerValues.ToString());

                if (match.Groups.Count == 2)
                {
                    return match.Groups[1].Value;
                }
            }

            return null;
        }

        private string WriteEmailBodyForProfile(Item mailItem, Profile profile)
        {
            StringBuilder builder = new StringBuilder();

            // Get a number only timestamp
            long time = DateTime.Now.ToFileTime() + DateTime.Now.Millisecond;

            // Save the file to the settings location with a filename of emailbody_{{timestamp}}.html
            string emailFilename = "emailbody_" + time + ".html";

            File.WriteAllText(profile.SavePath + emailFilename, mailItem.Body);
            
            foreach (Key k in profile.Keys)
            {
                builder.Append(k.ToDynamicString(profile.Delimiter, mailItem.Body));
            }

            // Append the file name to exporting string
            builder.Append(emailFilename);

            // Add a new line per individual item
            builder.Append("\r\n");

            return builder.ToString();
        }

        private string WriteAttachmentsForProfile(Item mailItem, Profile profile)
        {
            StringBuilder builder = new StringBuilder();

            // Get the attachments collection
            AttachmentCollection mailAttachments = mailItem.Attachments;

            foreach (FileAttachment attachment in mailAttachments)
            {
                // Load the attachment
                attachment.Load();

                string name = attachment.Name.Replace(" ", "").ToLower();
                name = DateTime.Now.ToFileTime() + DateTime.Now.Millisecond + "_" + name;

                // Save the attachment to the location defined in the settings
                File.WriteAllBytes(profile.SavePath + name, attachment.Content);
                
                // Foreach of the profile keys append the values to the export file
                foreach (Key k in profile.Keys)
                {
                    // Append the key value
                    builder.Append(k.ToDynamicString(profile.Delimiter, mailItem.Body));
                }

                // Append the filename of the attachment
                builder.Append(name);

                // Append a new line at the end of every item processed
                builder.Append("\r\n");
            }

            return builder.ToString();
        }

        private void MoveItemToFolder(Item mailItem, string folderName, FolderId folderId = null)
        {
            // If the folder id is null get the folder id by the folder name
            if (folderId == null)
                folderId = FindOrCreateFolderByName(folderName);

            // Move the message to the folder
            mailItem.Move(folderId);
        }

        private FolderId FindOrCreateFolderByName(string folderName, bool recurse = false)
        {
            // Get the top 100 folders for the email account
            FolderView folders = new FolderView(100);

            // Set the properties of the search
            folders.PropertySet = new PropertySet(BasePropertySet.IdOnly);
            folders.PropertySet.Add(FolderSchema.DisplayName);

            // Set the traversal type for the folders
            folders.Traversal = FolderTraversal.Deep;

            // Get the folders collection
            FindFoldersResults folderResults = _exService.FindFolders(WellKnownFolderName.Root, folders);

            // Loop through the folders until the correct one is found
            foreach (Folder f in folderResults)
            {
                // If the current folder's display name is equal to the passed folder value
                if (f.DisplayName == folderName)
                    // Return the id of the folder
                    return f.Id;
            }

            // If this is the second call to the function skip trying to create the folder again
            if (!recurse)
            {
                // Create a new folder with the excahnge service
                Folder newFolder = new Folder(_exService);

                // Set the new folder's display name to the passed folder string value
                newFolder.DisplayName = folderName;

                // Save the new folder with the parent being the Inbox
                newFolder.Save(WellKnownFolderName.Inbox);

                // Call this function again to get the newly created folder id
                FindOrCreateFolderByName(folderName, true);
            }

            // Return null if the folder is not real
            return null;
        }

        private static ExchangeVersion GetVersionFromString(string checkVersion)
        {
            // Set exchange version (from settings)
            ExchangeVersion version;

            switch (checkVersion.Replace(" ", "").ToUpper())
            {
                case "2007SP1":
                    version = ExchangeVersion.Exchange2007_SP1;
                    break;
                case "2010":
                    version = ExchangeVersion.Exchange2010;
                    break;
                case "2010SP1":
                    version = ExchangeVersion.Exchange2010_SP1;
                    break;
                case "2010SP2":
                    version = ExchangeVersion.Exchange2010_SP2;
                    break;
                case "2013":
                    version = ExchangeVersion.Exchange2013;
                    break;
                case "2013SP1":
                    version = ExchangeVersion.Exchange2013_SP1;
                    break;
                default:
                    version = ExchangeVersion.Exchange2010;
                    break;
            }

            return version;
        }
    }
}