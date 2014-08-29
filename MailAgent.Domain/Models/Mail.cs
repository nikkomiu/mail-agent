using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MailAgent.Domain.Models
{
    public class Mail
    {
        public int Polling { get; set; }
        public string ExchangeVersion { get; set; }
        public string Url { get; set; }

        public string UseWindowsCredentials { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        
        public string ErrorFolder { get; set; }
        public string SuccessFolder { get; set; }
    }
}
