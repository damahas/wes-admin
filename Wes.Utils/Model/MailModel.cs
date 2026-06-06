using System;
using System.Collections.Generic;
using System.Text;

namespace Wes.Utils.Model
{
    public class MailModel
    {
        public string MailHost { get; set; }
        public string MailPort { get; set; }
        public string MailAccount { get; set; }
        public string MailPassword { get; set; }
        public string TestMail { get; set; }
        public bool EnableSsl { get; set; }
    }
}
