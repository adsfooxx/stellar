using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Mail
{
    public class VerifyEmailSettings
    {
        public const string VerifyEmailSettingsKey = "VerifyEmailSettings";
        public string ReturnUrl {  get; set; }
        public string ReturnCheckEmailUrl { get; set; }
        public string ReturnCheckChangeEmailUrl { get; set; }
    }
}
