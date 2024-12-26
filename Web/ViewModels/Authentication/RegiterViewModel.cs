using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels.Authentication
{
    public class RegiterViewModel
    {

        [Required(ErrorMessage = "請輸入帳號")]
        [StringLength(50, ErrorMessage = "帳號最多50字元")]
        public string Account { get; set; }

        [Required(ErrorMessage = "請輸入密碼'")]
        public string Password { get; set; }

        [Required(ErrorMessage = "請輸入電子郵件")]
        [EmailAddress(ErrorMessage = "請輸入正確的電子郵件格式")]
        public string Email { get; set; }
        public string text { get; set; }
    }
}
