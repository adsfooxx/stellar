using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;

namespace Web.ViewModels.Authentication
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "請輸入帳號")]
        public string Account {  get; set; }
        [Required(ErrorMessage = "請輸入密碼")]
        [DataType(DataType.Password)]
        public string Password {  get; set; }

    }
}
