using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Mail_Agent_Service
{
    class Settings
    {
        private FileMan fManager;

        public Dictionary<string, string> General { get; set; }
        public Dictionary<string, Profile> Profiles { get; set; }

        public Settings()
        {
            General = new Dictionary<string, string>();

            fManager = FileMan.LocalFile("Settings.xml");
            fManager.Read();
        }

        public void Parse()
        {
            XmlReader xReader = XmlReader.Create(new StringReader(fManager.FileContents));

            string groupElement = string.Empty;
            string parentElement = string.Empty;
            string currentElement = string.Empty;

            while (xReader.Read())
            {
                switch (xReader.NodeType)
                {
                    case XmlNodeType.Element:
                        // Set the group if it is a general or profile group
                        if (xReader.Name == "General" || xReader.Name == "Profiles")
                        {
                            groupElement = xReader.Name;
                        }

                        if (xReader.Name == "Profile" && groupElement == "Profiles")
                        {
                            Profile.CreateFromXml(xReader.ReadSubtree());
                        }

                        // Set parent to current element if it is a general subgroup
                        if ((xReader.Name == "Mail" || xReader.Name == "Log") && groupElement == "General")
                        {
                            parentElement = xReader.Name;
                        }

                        currentElement = xReader.Name;
                        break;
                    case XmlNodeType.Text:
                        // If the element has a value save the value to parent + element name in dictionary
                        // Ex. Parent = Log, Element = Level, Dictionary Key = LogLevel
                        if (xReader.HasValue)
                        {
                            this.General[parentElement + currentElement] = xReader.Value;
                        }
                        break;
                    case XmlNodeType.EndElement:
                        break;
                    default:
                        break;
                }
            }

            General["MailPolling"] += "000";
        }
    }
}
