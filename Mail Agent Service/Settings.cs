using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Mail_Agent_Service
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
            FileMan fManager = FileMan.LocalFile("Settings.xml");
            fManager.Read();

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

                        if (xReader.Name == "Profiles")
                        {
                            Profiles.Add(Profile.CreateFromXml(xReader.ReadSubtree(), this.General["DefaultSavePath"]));
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
                            if (currentElement == "DefaultSavePath")
                                this.General[currentElement] = xReader.Value;
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

            fManager = null;
        }
    }
}
