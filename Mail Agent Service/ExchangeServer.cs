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

        public ExchangeServer(Dictionary<string, string> settings)
        {
            // TODO: Setup Exchange Version for Settings file

            this.exService = new ExchangeService(ExchangeVersion.Exchange2010);

            if (settings["MailUseWindowsCredentials"].ToUpper() == "TRUE")
            {
                exService.UseDefaultCredentials = true;
            }
            else
            {
                exService.UseDefaultCredentials = false;
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

            ErrorFolderName = settings["MailErrorFolder"];
        }

        public void SaveMail(List<Profile> Profiles, Logging log)
        {
            FindItemsResults<Item> inboxItems = exService.FindItems(WellKnownFolderName.Inbox, new ItemView(100));

            foreach (Profile profile in Profiles)
            {
                foreach (Item mailItem in inboxItems)
                {
                    try
                    {
                        if (mailItem.HasAttachments)
                        {
                            log.WriteLine(Logging.Level.INFO, mailItem.Subject);

                            // Load the item to get the body + attachments
                            mailItem.Load();

                            // Log the first line of the message
                            log.WriteLine(Logging.Level.INFO, " [body]: " + mailItem.Body.Text.ToString().Split('\n')[0]);

                            AttachmentCollection mailAttachments = mailItem.Attachments;

                            if (profile.SaveEmailBody)
                            {
                                long time = DateTime.Now.ToFileTime();
                                File.WriteAllText(profile.SavePath + "body_" + time + ".html", mailItem.Body);

                                log.WriteLine(Logging.Level.INFO, "Email body was written to " + profile.SavePath + " with the filename body_" + time + ".html");
                            }

                            if (profile.SaveAttachments)
                            {
                                foreach (FileAttachment attachment in mailAttachments)
                                {
                                    attachment.Load();

                                    // Save the attachment
                                    File.WriteAllBytes(profile.SavePath + attachment.Name, attachment.Content);

                                    log.WriteLine(Logging.Level.INFO, "Attachment " + attachment.Name + " was written to " + profile.SavePath);
                                }
                            }
                        }

                        mailItem.Move(WellKnownFolderName.DeletedItems);
                        log.WriteLine(Logging.Level.INFO, "Moved " + mailItem.Subject + " to Deleted Items folder");
                    }
                    catch (Exception ex)
                    {
                        // Log the error
                        log.WriteError(ex);

                        if (ErrorFolderId == null)
                        {
                            FolderView folders = new FolderView(100);
                            folders.PropertySet = new PropertySet(BasePropertySet.IdOnly);
                            folders.PropertySet.Add(FolderSchema.DisplayName);
                            folders.Traversal = FolderTraversal.Deep;

                            FindFoldersResults folderResults = exService.FindFolders(WellKnownFolderName.Root, folders);

                            foreach (Folder f in folderResults)
                            {
                                if (f.DisplayName == ErrorFolderName)
                                    ErrorFolderId = f.Id;
                            }
                        }

                        mailItem.Move(ErrorFolderId);
                    }
                }
            }
        }
    }
}
