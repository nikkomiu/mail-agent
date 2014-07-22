using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace MailAgent.Service
{
    class Settings
    {
        public Dictionary<string, string> General { get; set; }
        public List<Profile> Profiles { get; set; }

        public Settings()
        {
            General = new Dictionary<string, string>();
            Profiles = new List<Profile>();
        }

        public void Parse()
        {
            FileMan fileManager = FileMan.LocalFile("Settings.xml");
            fileManager.Read();

            XmlReader xmlReader = XmlReader.Create(new StringReader(fileManager.FileContents));

            string groupElement = string.Empty;
            string parentElement = string.Empty;
            string currentElement = string.Empty;

            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        // Set the group if it is a general or profile group
                        if (xmlReader.Name == "General" || xmlReader.Name == "Profiles")
                        {
                            groupElement = xmlReader.Name;
                        }

                        if (xmlReader.Name == "Profile")
                        {
                            Profile tmpProfile = Profile.CreateFromXml(xmlReader.ReadSubtree(), this.General["DefaultSavePath"], this.General["ExportKeyDelimiter"]);

                            if (tmpProfile.Active)
                            {
                                Profiles.Add(tmpProfile);
                            }
                        }

                        // Set parent to current element if it is a general subgroup
                        if ((xmlReader.Name == "Mail" || xmlReader.Name == "Log") && groupElement == "General" || xmlReader.Name == "Export")
                        {
                            parentElement = xmlReader.Name;
                        }

                        currentElement = xmlReader.Name;
                        break;
                    case XmlNodeType.Text:
                        // If the element has a value save the value to parent + element name in dictionary
                        // Ex. Parent = Log, Element = Level, Dictionary Key = LogLevel
                        if (xmlReader.HasValue)
                        {
                            if (currentElement == "DefaultSavePath")
                                this.General[currentElement] = xmlReader.Value;
                            this.General[parentElement + currentElement] = xmlReader.Value;
                        }
                        break;
                    case XmlNodeType.EndElement:
                        break;
                    default:
                        break;
                }
            }

            fileManager = null;
        }
    }
}
