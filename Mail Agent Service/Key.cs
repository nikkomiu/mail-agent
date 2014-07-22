using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailAgent.Service
{
    public class Key
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }

        private int _increment;
        
        public Key(string name = "", string value = "", string type = "")
        {
            _increment = 1;
            this.Name = name;
            this.Value = value;
            this.Type = type;
        }

        public string ToDynamicString(string delimiter, string dynamicSearch = "")
        {
            string returnString = string.Empty;

            if (this.Type == null)
                throw new NotSupportedException();

            if (this.Type == "Static")
            {
                returnString = this.Value;
            }
            else if (this.Type == "Dynamic")
            {
                if (this.Value == "DATE" || this.Value == "DATETIME")
                {
                    returnString += DateTime.Now.ToShortDateString();
                }

                if (this.Value == "TIME" || this.Value == "DATETIME")
                {
                    returnString += DateTime.Now.ToShortTimeString();
                }

                if (this.Value == "INCREMENT")
                {
                    returnString += _increment;
                    _increment++;
                }
            }
            else if (this.Type == "Search" && dynamicSearch.Length > 0)
            {
                this.KeySearch(dynamicSearch);
            }

            returnString += delimiter;

            return returnString;
        }

        private string KeySearch(string dynamicSearch)
        {
            string[] splitString = dynamicSearch.Split('\n');

            foreach (string line in splitString)
            {
                int index = line.IndexOf(this.Value);
                if (index > 0)
                {
                    return line.Substring(index + this.Value.Length);
                }
            }

            return string.Empty;
        }

        public override string ToString()
        {
            return this.Value;
        }
    }
}
