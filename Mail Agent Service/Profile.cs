using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Mail_Agent_Service
{
    public class Profile
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }

        public bool SaveAttachments { get; set; }
        public bool SaveEmailBody { get; set; }
        public string SavePath { get; set; }

        public Profile()
        {
            this.Id = string.Empty;

            this.EmailSubject = string.Empty;
            this.EmailBody = string.Empty;

            this.SaveAttachments = false;
            this.SaveEmailBody = false;
            this.SavePath = string.Empty;
        }

        public static Profile CreateFromXml(XmlReader xmlString, string defaultPath)
        {
            // Create a new profile
            Profile profile = new Profile();

            string currentElement = string.Empty;

            // Loop through the xml
            while (xmlString.Read())
            {
                switch (xmlString.NodeType)
                {
                    case XmlNodeType.Element:
                        currentElement = xmlString.Name;
                        break;
                    case XmlNodeType.Text:
                        if (currentElement == "Name")
                            profile.Name = xmlString.Value;
                        
                        if (currentElement == "EmailSubject")
                            profile.EmailSubject = xmlString.Value;

                        if (currentElement == "EmailBody")
                            profile.EmailBody = xmlString.Value;

                        if (currentElement == "SavePath")
                            profile.SavePath = xmlString.Value;

                        if (currentElement == "Id")
                            profile.Id = xmlString.Value;

                        if (currentElement == "SaveAttachments")
                        {
                            bool saveBool;
                            bool.TryParse(xmlString.Value, out saveBool);
                            profile.SaveAttachments = saveBool;
                        }

                        if (currentElement == "SaveBody")
                        {
                            bool saveBool;
                            bool.TryParse(xmlString.Value, out saveBool);
                            profile.SaveEmailBody = saveBool;
                        }

                        break;
                    case XmlNodeType.EndElement:
                        break;
                    default:
                        break;
                }
            }

            if (profile.SavePath.Length == 0)
                profile.SavePath = defaultPath;

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
