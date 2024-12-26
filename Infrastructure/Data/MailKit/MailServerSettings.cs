using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.MailKit
{
    public class MailServerSettings
    {
        //後面的等於單純只是因為class名稱跟我們的appsetting撞名，欄位沒辦法跟class同名才這樣帶的
        public const string MailServerSettingsKey = "MailServerSettings";
        public string Host { get; set; }
        public int Port { get; set; }
        public bool UseSsl { get; set; }
        public string DefaultSenderName { get; set; }
        public string OfficialEmail { get; set; }
        public string Password { get; set; }
    }
}
