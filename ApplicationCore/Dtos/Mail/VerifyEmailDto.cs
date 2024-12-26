using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Dtos.Mail
{
    public class VerifyEmailDto
    {
        public string Email { get; set; }
        public DateTime ExpireTime { get; set; }

    }
}
