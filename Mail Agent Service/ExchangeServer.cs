using Microsoft.Exchange.WebServices;
using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Mail_Agent_Service
{
    public class ExchangeServer
    {
        private ExchangeService exService;

        private FolderId ErrorFolderId;
        private string ErrorFolderName;

        private FolderId SuccessFolderId;
        private string SuccessFolderName;

        public ExchangeServer(Dictionary<string, string> settings)
        {
            // Set the version of Exchange from settings file
            this.exService = new ExchangeService(GetVersionFromString(settings["MailExchangeVersion"]));

            // If not using windows credentials (from settings file) use manually define credentials
            if (settings["MailUseWindowsCredentials"].ToUpper() == "TRUE")
            {
                // Use Windows Credentials
                exService.UseDefaultCredentials = true;
            }
            else
            {
                // Do not use Windows Credentials
                exService.UseDefaultCredentials = false;

                // Set the login credentials
                exService.Credentials = new NetworkCredential(settings["MailEmail"], settings["MailPassword"]);
            }


            // If URL is Auto
            if (settings["MailUrl"].ToUpper() == "AUTO")
            {
                // Autodiscover based on email address
                exService.AutodiscoverUrl(settings["MailEmail"]);
            }
            else
            {
                // Set the URL manually
                exService.Url = new Uri(settings["MailUrl"]);
            }

            // Set the error folder name from the settings
            ErrorFolderName = settings["MailErrorFolder"];
            SuccessFolderName = settings["MailSuccessFolder"];
        }

        public void SaveMail(List<Profile> Profiles, Logging log)
        {
            FindItemsResults<Item> inboxItems = exService.FindItems(WellKnownFolderName.Inbox, new ItemView(100));

            List<Item> completeInboxItems = new List<Item>();

            foreach (Profile profile in Profiles)
            {
                foreach (Item mailItem in inboxItems)
                {
                    try
                    {
                        // TODO: Make sure the email should be used by this profile

                        // Log the subject of the email being processed
                        log.WriteLine(Logging.Level.DEBUG, mailItem.Subject);

                        // Load the item to get the body + attachments
                        mailItem.Load();

                        // If the profile has an email subject to match then check the email subject for the string
                        if (profile.EmailSubject.Length > 0)
                        {
                            // If the email subject does not contain the profile subject skip the current email
                            if (!mailItem.Subject.Contains(profile.EmailSubject))
                            {
                                continue;
                            }
                        }

                        // If the profile has an email body to match then check the email body for the string
                        if (profile.EmailBody.Length > 0)
                        {
                            // If the email body does not contain the profile body skip the current email
                            if (!mailItem.Body.Text.Contains(profile.EmailBody))
                            {
                                continue;
                            }
                        }

                        // Save the body if the profile wants it saved
                        if (profile.SaveEmailBody)
                        {
                            // Get a number only timestamp
                            long time = DateTime.Now.ToFileTime();
                            
                            // Save the file to the settings location with a filename of emailbody_{{timestamp}}.html
                            File.WriteAllText(profile.SavePath + "emailbody_" + time + ".html", mailItem.Body);
                            log.WriteLine(Logging.Level.INFO, "Email body was written to " + profile.SavePath + " with the filename body_" + time + ".html");
                        }

                        // Save the attachments if the profile wants them saved
                        if (mailItem.HasAttachments && profile.SaveAttachments)
                        {
                            // Get the attachments collection
                            AttachmentCollection mailAttachments = mailItem.Attachments;

                            foreach (FileAttachment attachment in mailAttachments)
                            {
                                // Load the attachment
                                attachment.Load();

                                // Save the attachment to the location defined in the settings
                                File.WriteAllBytes(profile.SavePath + attachment.Name, attachment.Content);
                                log.WriteLine(Logging.Level.INFO, "Attachment " + attachment.Name + " was written to " + profile.SavePath);
                            }
                        }

                        // Move the email to the deleted items folder
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

                    completeInboxItems.Add(mailItem);
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

        private void MoveItemToFolder(Item mailItem, string folderName, FolderId folderId = null)
        {
            // If the folder id is null get the folder id by the folder name
            if (folderId == null)
                folderId = GetFolderByName(folderName);

            // Move the message to the folder
            mailItem.Move(folderId);
        }

        private FolderId GetFolderByName(string folderName)
        {
            // Get the top 100 folders for the email account
            FolderView folders = new FolderView(100);

            // Set the properties of the search
            folders.PropertySet = new PropertySet(BasePropertySet.IdOnly);
            folders.PropertySet.Add(FolderSchema.DisplayName);

            // Set the traversal type for the folders
            folders.Traversal = FolderTraversal.Deep;

            // Get the folders collection
            FindFoldersResults folderResults = exService.FindFolders(WellKnownFolderName.Root, folders);

            // Loop through the folders until the correct one is found
            foreach (Folder f in folderResults)
            {
                if (f.DisplayName == folderName)
                    return f.Id;
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
