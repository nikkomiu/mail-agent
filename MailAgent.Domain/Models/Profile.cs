using System;
using System.Xml;
using System.Xml.Serialization;

namespace MailAgent.Domain.Models
{
    public class Profile
    {
        public Guid ID { get; set; }

        public string Active { get; set; }

        public string Name { get; set; }

        public string Alias { get; set; }

        public string EmailSubject { get; set; }

        public string EmailBody { get; set; }
        
        [XmlElement(ElementName="SaveEmailBody")]
        public string SaveAsString
        {
            get
            {
                return XmlConvert.ToString(SaveEmailBody);
            }
            set
            {
                bool parsedValue;

                bool.TryParse(value, out parsedValue);

                SaveEmailBody = parsedValue;
            }
        }

        [XmlIgnore]
        public bool SaveEmailBody { get; set; }

        [XmlElement(ElementName="SaveAttachments")]
        public string SaveAttachmentsAsString
        {
            get
            {
                return XmlConvert.ToString(SaveAttachments);
            }
            set
            {
                bool parsedValue;

                bool.TryParse(value, out parsedValue);

                SaveAttachments = parsedValue;
            }
        }

        [XmlIgnore]
        public bool SaveAttachments { get; set; }

        public string SavePath { get; set; }

        [XmlAttribute(AttributeName="UsesDefaultAttachment")]
        public string IsDefaultSavePathAsString
        {
            get
            {
                return XmlConvert.ToString(IsDefaultSavePath);
            }
            set
            {
                bool parsedValue;

                bool.TryParse(value, out parsedValue);

                IsDefaultSavePath = parsedValue;
            }
        }

        [XmlIgnore]
        public bool IsDefaultSavePath { get; set; }

        public string KeyDelimiter { get; set; }

        [XmlAttribute(AttributeName="UsesDefaultDelimiter")]
        public string IsDefaultKeyDelimiterAsString
        {
            get
            {
                return XmlConvert.ToString(IsDefaultKeyDelimiter);
            }
            set
            {
                bool parsedValue;

                bool.TryParse(value, out parsedValue);

                IsDefaultKeyDelimiter = parsedValue;
            }
        }
    
        [XmlIgnore]
        public bool IsDefaultKeyDelimiter { get; set; }

        public Key[] Keys { get; set; }
    }
}
