using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.Exchange.WebServices.Data;

namespace Mail_Agent_Service
{
    public class ExchangeServer
    {
        private ExchangeService ExService;

        private FolderId ErrorFolderId;
        private string ErrorFolderName;

        private FolderId SuccessFolderId;
        private string SuccessFolderName;

        private string ExportFilename;
        private string GlobalSavePath;

        public ExchangeServer(Dictionary<string, string> settings)
        {
            // Set the version of Exchange from settings file
            this.ExService = new ExchangeService(GetVersionFromString(settings["MailExchangeVersion"]));

            // If not using windows credentials (from settings file) use manually define credentials
            if (settings["MailUseWindowsCredentials"].ToUpper() == "TRUE")
            {
                // Use Windows Credentials
                ExService.UseDefaultCredentials = true;
            }
            else
            {
                // Do not use Windows Credentials
                ExService.UseDefaultCredentials = false;

                // Set the login credentials
                ExService.Credentials = new NetworkCredential(settings["MailEmail"], settings["MailPassword"]);
            }


            // If URL is Auto
            if (settings["MailUrl"].ToUpper() == "AUTO")
            {
                // Autodiscover based on email address
                ExService.AutodiscoverUrl(settings["MailEmail"]);
            }
            else
            {
                // Set the URL manually
                ExService.Url = new Uri(settings["MailUrl"]);
            }

            // Set the error folder name from the settings
            ErrorFolderName = settings["MailErrorFolder"];
            SuccessFolderName = settings["MailSuccessFolder"];
            ExportFilename = settings["ExportFilename"];
            GlobalSavePath = settings["DefaultSavePath"];
        }

        public void SaveMail(List<Profile> Profiles, Logging log)
        {
            // Get all of the mailbox items
            FindItemsResults<Item> inboxItems = ExService.FindItems(WellKnownFolderName.Inbox, new ItemView(100));

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

                        // If the profile has an email subject to match then check the email subject for the string
                        // and if the email subject does not contain the profile subject skip the current email
                        if (profile.EmailSubject.Length > 0 && !mailItem.Subject.Contains(profile.EmailSubject))
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
                        MoveItemToFolder(mailItem, SuccessFolderName, SuccessFolderId);
                        log.WriteLine(Logging.Level.INFO, "Moved " + mailItem.Subject + " to " + SuccessFolderName + " folder");
                    }
                    catch (Exception ex)
                    {
                        // Log the error
                        log.WriteError(ex);

                        // Move the email to the error folder
                        MoveItemToFolder(mailItem, ErrorFolderName, ErrorFolderId);
                    }

                    // Add the item to the complete list
                    completeInboxItems.Add(mailItem);
                }

                // Write the export file if there were emails processed and completed
                if (completeInboxItems.Count > 0 && localExportText.Length > 0)
                {
                    // Write the export file to the profile's save path
                    File.WriteAllText(profile.SavePath + DateTime.Now.ToFileTime() + "_" + ExportFilename, localExportText);
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
                MoveItemToFolder(mailItem, ErrorFolderName, ErrorFolderId);
            }
        }

        private string WriteEmailBodyForProfile(Item mailItem, Profile profile)
        {
            StringBuilder builder = new StringBuilder();

            // Get a number only timestamp
            long time = DateTime.Now.ToFileTime();

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

                // Remove spaces and set filename to lowercase
                string attachmentName = attachment.Name.ToLower().Replace(" ", "");

                int extensionPosition = attachmentName.LastIndexOf(".");

                // Get the extension
                string attachmentExtension = attachmentName.Substring(extensionPosition);

                // Rebuild the extension
                attachmentName = attachmentName.Remove(extensionPosition) + "_" + DateTime.Now.ToFileTime() + attachmentExtension;

                // Save the attachment to the location defined in the settings
                File.WriteAllBytes(profile.SavePath + attachmentName, attachment.Content);
                
                // Foreach of the profile keys append the values to the export file
                foreach (Key k in profile.Keys)
                {
                    // Append the key value
                    builder.Append(k.ToDynamicString(profile.Delimiter, mailItem.Body));
                }

                // Append the filename of the attachment
                builder.Append(attachmentName);

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
            FindFoldersResults folderResults = ExService.FindFolders(WellKnownFolderName.Root, folders);

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
                Folder newFolder = new Folder(ExService);

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
