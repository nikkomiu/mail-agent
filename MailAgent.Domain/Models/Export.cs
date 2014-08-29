using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MailAgent.Domain.Models
{
    public class Export
    {
        public string Filename { get; set; }
        public string KeyDelimiter { get; set; }
    }
}
