using System.ComponentModel.DataAnnotations;

namespace Stellar.API.Dtos.JWT
{
    public class LoginDto
    {
        [Required] // 確保使用了必要的特性來標記為必填
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
