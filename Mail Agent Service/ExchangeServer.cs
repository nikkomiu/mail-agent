using Microsoft.Exchange.WebServices;
using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Net;

namespace Mail_Agent_Service
{
    public class ExchangeServer
    {
        private ExchangeService exService;

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
        }

        public void GetMail(List<Profile> Profiles, Logging log)
        {
            FindItemsResults<Item> inboxItems = exService.FindItems(WellKnownFolderName.Inbox, new ItemView(100));

            foreach (Item mailItem in inboxItems)
            {
                if (mailItem.HasAttachments)
                {
                    log.WriteLine(Logging.Level.INFO, mailItem.Subject);

                    // Load the item to get the body + attachments
                    mailItem.Load();

                    // Log the first line of the message
                    log.WriteLine(Logging.Level.INFO, " -[body]: " + mailItem.Body.Text.ToString().Split('\n')[0]);

                    AttachmentCollection mailAttachments = mailItem.Attachments;
                    
                    foreach (Attachment attachment in mailAttachments)
                    {
                        log.WriteLine(Logging.Level.INFO, " --[attachment]: " + attachment.Name);
                    }
                }
            }
        }
    }
}
