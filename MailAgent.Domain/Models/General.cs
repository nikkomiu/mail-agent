using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MailAgent.Domain.Models
{
    public class General
    {
        public Log Log { get; set; }
        public Mail Mail { get; set; }

        public Export Export { get; set; }

        public string DefaultSavePath { get; set; }
    }
}
