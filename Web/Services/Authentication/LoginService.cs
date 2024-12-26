using ApplicationCore.Interfaces;
using CloudinaryDotNet;
using Google.Apis.Auth;
using NuGet.Protocol;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using Web.Helpers;
using Web.Models;
using Web.Service.Layout;
using Web.ViewModels.Authentication;
using Web.ViewModels.Layout;
using Web.ViewModels.Member;
namespace Web.Services.Authentication
{
    public class LoginService
    {
        private readonly IRepository<User> _userRepository;
        private readonly LayoutService _layoutService;

        public LoginService(IRepository<User> userPepository, LayoutService layoutService)
        {
            _userRepository = userPepository;
            _layoutService = layoutService;
        }

        public async Task<User> FindOrCreateUser(GoogleJsonWebSignature.Payload payload)
        {
            
            var user = await _userRepository.FirstOrDefaultAsync(u => u.EmailAddress == payload.Email);

            if (user == null)
            {

                user = new User
                {
                    EmailAddress = payload.Email,
                    NickName = payload.Name,
                    Account = payload.Email,
                    Password = GenerateRandomHash(),
                    WalletAmount = 0, 
                    UserImg = payload.Picture,
                    Country = "台灣",
                    City = "台北市",
                };
                await _userRepository.AddAsync(user);
            }

            return user;

        }

        //隨機雜湊密碼給Gmail第三方登入
        private string GenerateRandomHash()
        {
            using var sha256 = SHA256.Create();
            var randomBytes = new byte[32]; 
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes); 
            var hashBytes = sha256.ComputeHash(randomBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        public async Task<int> UserLogin(LoginViewModel user)
        {
            if (user.Account != null && user.Password != null)
            {
                var password = Hash256.GetSHA256(user.Password);
                bool userHas = await _userRepository.AnyAsync(x => x.Account == user.Account && x.Password == password && x.IsLocked == 1);
                bool isLocked = await _userRepository.AnyAsync(x => x.Account == user.Account && x.Password == password && x.IsLocked == 0);
                if (userHas)
                {
                    return _userRepository.FirstOrDefault(x => x.Account == user.Account && x.Password == password).UserId;
                }

                if (isLocked)
                {
                    return -2;
                }

            }



            return -1;
        }


    }
}
