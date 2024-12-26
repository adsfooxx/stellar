using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Dtos.Authentication
{
    public class ChangePasswordDto
    {
        
        public required string CurrentPassword { get; set; } 
        public required string NewPassword { get; set; }
        public required string CheckNewPassword {  get; set; }
    }
}
