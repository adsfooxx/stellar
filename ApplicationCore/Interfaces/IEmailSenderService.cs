﻿using ApplicationCore.Dtos.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IEmailSenderService
    {
        public  Task SendEmailAsync(SendEmailRequest request);
    }
}
