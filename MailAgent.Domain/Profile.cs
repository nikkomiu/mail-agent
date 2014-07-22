using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MailAgent.Domain
{
    public class Profile
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }

        public string Alias { get; set; }

        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }

        public bool SaveAttachments { get; set; }
        public bool SaveEmailBody { get; set; }
        public string SavePath { get; set; }
        public bool IsDefaultPath { get; set; }

        public string Delimiter { get; set; }

        public List<Key> Keys { get; set; }

        public Profile()
        {
            this.Id = string.Empty;

            this.EmailSubject = string.Empty;
            this.EmailBody = string.Empty;

            this.SaveAttachments = false;
            this.SaveEmailBody = false;
            this.SavePath = string.Empty;
            this.IsDefaultPath = false;

            this.Delimiter = string.Empty;

            this.Active = false;

            this.Keys = new List<Key>();
        }

        public static Profile CreateFromXml(XmlReader xmlReader, string defaultPath, string delimiterParam)
        {
            // Create a new profile
            Profile profile = new Profile();

            string currentElement = string.Empty;
            string parentElement = string.Empty;
            string typeAttribute = string.Empty;

            // Loop through the xml
            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (xmlReader.Name == "EmailBody")
                        {
                            bool saveBool;
                            bool.TryParse(xmlReader.GetAttribute("Save"), out saveBool);
                            profile.SaveEmailBody = saveBool;
                        }

                        if (xmlReader.Name == "Keys")
                            parentElement = xmlReader.Name;

                        if (parentElement == "Keys")
                        {
                            typeAttribute = xmlReader.GetAttribute("Type");
                        }

                        currentElement = xmlReader.Name;
                        break;
                    case XmlNodeType.Text:
                        if (currentElement == "Name")
                            profile.Name = xmlReader.Value;
                        
                        if (currentElement == "EmailSubject")
                            profile.EmailSubject = xmlReader.Value;

                        if (currentElement == "EmailBody")
                            profile.EmailBody = xmlReader.Value;

                        if (currentElement == "SavePath")
                            profile.SavePath = xmlReader.Value;

                        if (currentElement == "Id")
                            profile.Id = xmlReader.Value;

                        if (currentElement == "KeyDelimiter")
                            profile.Delimiter = xmlReader.Value;

                        if (currentElement == "Active")
                        {
                            bool activeBool = false;
                            bool.TryParse(xmlReader.Value, out activeBool);
                            profile.Active = activeBool;
                        }

                        if (currentElement == "SaveAttachments")
                        {
                            bool saveBool;
                            bool.TryParse(xmlReader.Value, out saveBool);
                            profile.SaveAttachments = saveBool;
                        }

                        if (parentElement.Length > 0 && parentElement != currentElement)
                        {
                            profile.Keys.Add(new Key(currentElement, xmlReader.Value, typeAttribute));
                        }

                        break;
                    case XmlNodeType.EndElement:
                        break;
                    default:
                        break;
                }
            }

            if (profile.Delimiter.Length == 0)
            {
                profile.Delimiter = delimiterParam;
            }

            if (profile.SavePath.Length == 0)
            {
                profile.SavePath = defaultPath;
                profile.IsDefaultPath = true;
            }
                
            if (profile.Id.Length == 0)
            {
                profile.Id = (Path.GetRandomFileName().Replace('.', '_') +
                    Path.GetRandomFileName().Split('.')[0] + "_" +
                    profile.Name.GetHashCode()).ToUpper();

                // TODO: Save the new ID for the profile
            }

            return profile;
        }
    }
}
