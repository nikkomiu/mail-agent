using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailAgent.Domain.Models
{
    public class Settings
    {
        public General General { get; set; }
        public Profile[] Profiles { get; set; }
    }
}
