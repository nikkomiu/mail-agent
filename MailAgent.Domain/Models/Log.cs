using System;
using System.Xml;
using System.Xml.Serialization;

namespace MailAgent.Domain.Models
{
    public class Log
    {
        [XmlIgnore]
        public bool LocalLocation { get; set; }

        [XmlElement(ElementName="LocalLocation")]
        public string LocalLocationAsString
        {
            get
            {
                return XmlConvert.ToString(LocalLocation);
            }
            set
            {
                bool parseValue;

                bool.TryParse(value, out parseValue);

                LocalLocation = parseValue;
            }
        }

        public string Location { get; set; }
        public MailAgent.Domain.Logging.Level Level { get; set; }
    }
}
