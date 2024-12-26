using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Dtos.Mail
{
    public class SendEmailRequest
    {
        public string Receiver { get; set; }
        public string ReceiverEmail { get; set; }
        public string? SenderName { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
