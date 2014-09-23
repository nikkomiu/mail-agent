using MailAgent.Domain;
using MailAgent.Domain.Models;
using Microsoft.Exchange.WebServices.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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

        /// <summary>
        /// Parameterized constructor for the class that allows for
        /// the logging object to be passed in allowing for logging of information
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="_loggerger"></param>
        public ExchangeServer(General settings, Logging logger) : this(settings)
        {
            this._logger = logger;
        }

        /// <summary>
        /// Parameterized constructor for the class
        /// Sets up the basic information required to 
        /// </summary>
        /// <param name="settings"></param>
        public ExchangeServer(General settings)
        {
            // Set the version of Exchange from settings file
            this._exService = new ExchangeService(GetVersionFromString(settings.Mail.ExchangeVersion));

            _emailAddress = settings.Mail.Email;

            // If not using windows credentials (from settings file) use manually define credentials
            if (settings.Mail.UseWindowsCredentials.ToUpper() == "TRUE")
            {
                // Use Windows Credentials
                _exService.UseDefaultCredentials = true;
            }
            else
            {
                // Do not use Windows Credentials
                _exService.UseDefaultCredentials = false;

                // Set the _loggerin credentials
                _exService.Credentials = new NetworkCredential(_emailAddress, settings.Mail.Password);
            }

            // If URL is Auto
            if (settings.Mail.Url.ToUpper() == "AUTO")
            {
                // Autodiscover based on email address
                _exService.AutodiscoverUrl(_emailAddress);
            }
            else
            {
                // Set the URL manually
                _exService.Url = new Uri(settings.Mail.Url);
            }

            // Set the error folder name from the settings
            _errorFolderName = settings.Mail.ErrorFolder;
            _successFolderName = settings.Mail.SuccessFolder;
            _exportFilename = settings.Export.Filename;
            _globalSavePath = settings.DefaultSavePath;
        }

        /// <summary>
        /// The beginning of the entire program loop.
        /// Gets all of the emails in the inbox and loops through
        /// them checking of there is a profile that matches the email
        /// </summary>
        /// <param name="profiles"></param>
        public void SaveMail(Profile[] profiles)
        {
            // Get all of the mailbox items
            FindItemsResults<Item> inboxItems = _exService.FindItems(WellKnownFolderName.Inbox, new ItemView(100));

            // List of mailbox items that have been processed
            List<Item> completeInboxItems = new List<Item>();

            foreach (Item item in inboxItems)
            {
                MatchItemToProfile(item, profiles);
            }
        }

        /// <summary>
        /// Given the current email and profiles this should
        /// loop through the array of profiles trying to find
        /// the profile that matches the current email
        /// </summary>
        /// <param name="item"></param>
        /// <param name="profiles"></param>
        private void MatchItemToProfile(Item item, Profile[] profiles)
        {
            string localExportText = string.Empty;

            bool foundResult = false;

            foreach (Profile profile in profiles)
            {
                localExportText = ItemProfileCheck(item, profile);

                if (!string.IsNullOrEmpty(localExportText))
                {
                    foundResult = true;

                    // Write the export file to the profile's save path
                    File.WriteAllText(profile.SavePath + DateTime.Now.ToFileTime() + "_" + _exportFilename, localExportText);

                    break;
                }
            }

            if (!foundResult)
            {
                // Move the email to the error folder
                MoveItemToFolder(item, _errorFolderName, _errorFolderId);
            }
        }

        /// <summary>
        /// Check to see if the item and the profile match,
        /// if they do save the files and attachments
        /// (if the profile wants them saved)
        /// </summary>
        /// <param name="item"></param>
        /// <param name="profile"></param>
        /// <returns>
        /// The output string of index keys for the email
        /// </returns>
        private string ItemProfileCheck(Item item, Profile profile)
        {
            string localExportText = string.Empty;

            try
            {
                // Log the subject of the email being processed
                _logger.WriteLine(Logging.Level.DEBUG, "Current Item (Subject): " + item.Subject);

                // Load the item to get the body + attachments
                item.Load();

                // If the profile has an alias set and the email is not to the alias address
                // skip the current email
                if (!string.IsNullOrEmpty(profile.Alias))
                {
                    if (!CheckForAlias(item, profile.Alias))
                    {
                        _logger.WriteLine(Logging.Level.DEBUG, "Profile " + profile.Name + " does not have an alias for the email: " + item.Subject);
                        return string.Empty;
                    }
                }

                // If the profile has an email subject to match then check the email subject for the string
                // and if the email subject does not contain the profile subject skip the current email
                if (!string.IsNullOrEmpty(profile.EmailSubject) && !item.Subject.Contains(profile.EmailSubject))
                {
                    _logger.WriteLine(Logging.Level.DEBUG, "Profile " + profile.Name + " does not match the subject line for the email: " + item.Subject);
                    return string.Empty;
                }

                // If the profile has an email body to match then check the email body for the string
                // and if the email body does not contain the profile body skip the current email
                if (profile.EmailBody.Length > 0 && !item.Body.Text.Contains(profile.EmailBody))
                {
                    _logger.WriteLine(Logging.Level.DEBUG, "Profile " + profile.Name + " does not match the body string for the email: " + item.Subject);
                    return string.Empty;
                }

                _logger.WriteLine(Logging.Level.INFO, "Email is using the profile " + profile.Name);

                int totalItemsDropped = 0;

                // Save the body if the profile wants it saved
                if (profile.SaveEmailBody)
                {
                    localExportText += WriteEmailBodyForProfile(item, profile);
                    totalItemsDropped += 1;
                }

                // Save the attachments if the profile wants them saved
                if (item.HasAttachments && profile.SaveAttachments)
                {
                    localExportText += WriteAttachmentsForProfile(item, profile);
                    totalItemsDropped += item.Attachments.Count;
                }

                // Move the email to the success folder
                MoveItemToFolder(item, _successFolderName, _successFolderId);

                _logger.WriteLine(Logging.Level.INFO, totalItemsDropped + " attachment(s) dropped into folder");
                _logger.WriteLine(Logging.Level.INFO, "Moved `" + item.Subject + "` to " + _successFolderName + " folder using `" + profile.Name + "` profile");
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.WriteError(ex);

                if (ex.GetType() == new System.UnauthorizedAccessException().GetType())
                {
                    _logger.WriteLine(Logging.Level.CRITICAL, "------------ Friendly Error ------------");
                    _logger.WriteLine(Logging.Level.CRITICAL, "  The service is probably being ran as");
                    _logger.WriteLine(Logging.Level.CRITICAL, "  the wrong user, make sure the user");
                    _logger.WriteLine(Logging.Level.CRITICAL, "  can access the path provided");
                    _logger.WriteLine(Logging.Level.CRITICAL, "----------------------------------------");
                }
            }

            _logger.WriteLine(Logging.Level.INFO, "Key File Data: \r\n" + localExportText);

            return localExportText;
        }

        /// <summary>
        /// Try to find the given alias in the email headers
        /// </summary>
        /// <param name="mailItem">
        /// The current Email Item
        /// </param>
        /// <param name="alias">
        /// The email alias specified in the profile
        /// </param>
        /// <returns>
        /// True if the alias was found in the headers
        /// False if the alias was not found
        /// </returns>
        private bool CheckForAlias(Item mailItem, string alias)
        {
            ExtendedPropertyDefinition propertyDefinition = new ExtendedPropertyDefinition(0x007D, MapiPropertyType.String);
            PropertySet propertySet = new PropertySet(BasePropertySet.FirstClassProperties) { propertyDefinition, ItemSchema.MimeContent };

            mailItem.Load(propertySet);
            Object headerValues;
            if (mailItem.TryGetProperty(propertyDefinition, out headerValues))
            {
                string headerString = headerValues.ToString();

                // Split the lines on newline char and find the line(s) that contain the alias
                if (headerString.ToLower().IndexOf(alias) != -1)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Write the body of the email to the folder given in
        /// the settings file for the current profile.
        /// </summary>
        /// <param name="mailItem">
        /// The current email item
        /// </param>
        /// <param name="profile">
        /// The profile that is being used to
        /// write the email body to the output folder
        /// </param>
        /// <returns>
        /// A delimited string of the index keys
        /// and file location for the attachments
        /// </returns>
        private string WriteEmailBodyForProfile(Item mailItem, Profile profile)
        {
            StringBuilder builder = new StringBuilder();

            // Get a number only timestamp
            long time = DateTime.Now.ToFileTime() + DateTime.Now.Millisecond;

            // Save the file to the settings location with a filename of emailbody_{{timestamp}}.html
            string emailFilename = "emailbody_" + time + ".html";

            // Write the email body to the profile's path
            File.WriteAllText(profile.SavePath + emailFilename, mailItem.Body);

            // Get the JSON Key Overrides from the email body
            JSONKeys keyOverrideList = GetJsonKeyOverrideList(mailItem.Body);

            foreach (Key k in profile.Keys)
            {
                builder.Append(BuildIndexKeyString(k, profile.KeyDelimiter, mailItem.Body, keyOverrideList));
            }

            // Append the file name to exporting string
            builder.Append(emailFilename);

            // Add a new line per individual item
            builder.Append("\r\n");

            return builder.ToString();
        }

        /// <summary>
        /// Writes the file attachments of the email to the folder
        /// that is given in the settings file for the current profile.
        /// </summary>
        /// <param name="mailItem">
        /// The current email item
        /// </param>
        /// <param name="profile">
        /// The profile being used to write the
        /// attachments to the folder
        /// </param>
        /// <returns>
        /// A delimited string of the index keys
        /// and file location for the attachments
        /// </returns>
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

                // Get the JSON Key Overrides from the email body
                JSONKeys keyOverrideList = GetJsonKeyOverrideList(mailItem.Body);

                // Foreach of the profile keys append the values to the export file
                foreach (Key k in profile.Keys)
                {
                    // Append the key value
                    builder.Append(BuildIndexKeyString(k, profile.KeyDelimiter, mailItem.Body, keyOverrideList));
                }

                // Append the filename of the attachment
                builder.Append(name);

                // Append a new line at the end of every item processed
                builder.Append("\r\n");
            }

            return builder.ToString();
        }

        /// <summary>
        /// Get the JSON Key override list from the email body.
        /// The email body gets parsed for a script tag with type 'application/json'
        /// to get the list of override keys in JSON format
        /// </summary>
        /// <param name="messageBody">
        /// The string representation of the email body
        /// </param>
        /// <returns>
        /// A list of keys to override or an empty list of keys
        /// </returns>
        private JSONKeys GetJsonKeyOverrideList(MessageBody messageBody)
        {
            // Get Body JSON Data
            Match match = Regex.Match(messageBody, "<script .*type=?(\"|')application/json?(\"|').*?>.*?(\\[.*\\]).*?</script>", RegexOptions.Singleline);

            // Log the results of the JSON Match
            foreach (Group x in match.Groups)
            {
                _logger.WriteLine(Logging.Level.DEBUG, "REGEX Matching Group: " + x.Value);
            }

            try
            {
                string jsonString = match.Groups[match.Groups.Count - 1].Value;

                jsonString = "{\"KeyOverrides\":" + jsonString + "}";

                _logger.WriteLine(Logging.Level.DEBUG, "JSON String Matching: " + jsonString);

                // Return the deserialize the keys
                return JsonConvert.DeserializeObject<JSONKeys>(jsonString);
            }
            catch (Exception ex)
            {
                // Write the exception to the log
                _logger.WriteError(ex);

                // Return null (couldn't convert)
                return null;
            }
        }

        /// <summary>
        /// Builds the index key string based on the
        /// key, delimiter and email body. Also allows
        /// for the JSONKeys to override the current key value.
        /// </summary>
        /// <param name="key">
        /// The key to build the index string of
        /// </param>
        /// <param name="delimiter">
        /// The delimiter to seperate the key's value
        /// from the next key
        /// </param>
        /// <param name="emailBody">
        /// The string representation of the email body
        /// </param>
        /// <returns>
        /// The string representation of the index key value
        /// </returns>
        private string BuildIndexKeyString(Key key, string delimiter, string emailBody, JSONKeys keyOverrideList = null)
        {
            string keyValue = string.Empty;
            Key overrideKey = null;

            // If the key override exists
            if (keyOverrideList != null)
            {
                // Get the override key that has the same name if it exists
                overrideKey = keyOverrideList.KeyOverrides.Where(x => x.Name == key.Name).SingleOrDefault();
            }

            // If there is an override key
            if (overrideKey != null)
            {
                // If the override key type is null
                if (overrideKey.Type == null)
                {
                    // Set the override key type to the default key type
                    overrideKey.Type = key.Type;
                }

                // If the override key value is null
                if (overrideKey.Value == null)
                {
                    // Set the override key value to the default key value
                    overrideKey.Value = key.Value;
                }

                keyValue = overrideKey.ToDynamicString(delimiter, emailBody);
            }
            else
            {
                // Get the default key dynamic string
                keyValue = key.ToDynamicString(delimiter, emailBody);
            }

            return keyValue;
        }

        /// <summary>
        /// Move the mail item to the folder, if the folder id is null
        /// then it will get the folder id of the given folder name.
        /// </summary>
        /// <param name="mailItem">
        /// The current email item
        /// </param>
        /// <param name="folderName">
        /// The name of the folder that the email needs
        /// to be moved into
        /// </param>
        /// <param name="folderId">
        /// The Exchange Folder ID of the folder that the
        /// email item needs to be moved into (can be null)
        /// </param>
        private void MoveItemToFolder(Item mailItem, string folderName, FolderId folderId = null)
        {
            // If the folder id is null get the folder id by the folder name
            if (folderId == null)
            {
                folderId = FindOrCreateFolderByName(folderName);
            }

            // Move the message to the folder
            mailItem.Move(folderId);
        }

        /// <summary>
        /// Finds or creates the folder with the given name
        /// </summary>
        /// <param name="folderName">
        /// The name of the folder
        /// </param>
        /// <param name="recurse">
        /// Used to check if the call is a recursive call
        /// </param>
        /// <returns>
        /// The ID of the folder that is currently
        /// being referenced by name
        /// </returns>
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

        /// <summary>
        /// Exchange version string match for the string name in the settings file
        /// </summary>
        /// <param name="checkVersion">
        /// The string version to convert into the
        /// ExchangeVersion enum
        /// </param>
        /// <returns>
        /// The ExchangeVersion num (which API level to use)
        /// </returns>
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