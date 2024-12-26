using ApplicationCore.Dtos.Mail;
using ApplicationCore.Interfaces;
using Infrastructure.Data.Mail;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text;
using Web.Helpers;
using Web.Models;
using Web.ViewModels.Authentication;
using ApplicationCore.Dtos.Authentication;
using Infrastructure.Data.MailKit;
using Infrastructure.Services.MailKit;
using System.Web;
using Web.ViewModels.Member;


namespace Web.Services.Authentication
{
    public class RegiterService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<VerifyMail> _verifyMailRepository;
        private readonly IEmailSenderService _emailSenderService;
        private VerifyEmailSettings _verifyEmailSettings;
        public RegiterService(IRepository<User> userRepository, VerifyEmailSettings verifyEmailSettings, IRepository<VerifyMail> verifyMailRepository, IEmailSenderService emailSenderService)
        {
            _userRepository = userRepository;
            _verifyEmailSettings = verifyEmailSettings;
            _verifyMailRepository = verifyMailRepository;
            _emailSenderService = emailSenderService;
        }




        public async Task<User> UserLoginByLineLogin(LineLoginViewModel user)
        {

            var existingUser = _userRepository.SingleOrDefault(u => u.EmailAddress == user.Email && u.Account == user.LineId);


            if (existingUser is not null)
            {

                return existingUser;
            }


            User lineUser = new()
            {
                NickName = user.NickName,
                EmailAddress = user.Email,
                Password = Hash256.GetSHA256(Guid.NewGuid().ToString("N")),
                Account = user.LineId,
                UserImg = user.ImgUrl,
                WalletAmount = 0,
                Country = "台灣",
                City = "台北市",
                Overview = "沒有輸入任何資料",     
                State = 0,
                Online = 1,
                IsLocked = 1,
            };


            await _userRepository.AddAsync(lineUser);

            return lineUser;
        }


        public async Task CreatViewModelToDataModel(RegiterViewModel input)
        {
            string inputAccount = input.Account;
            if (_userRepository.Any(x => x.Account == inputAccount))
            {
                throw new Exception("帳號重複");
            }
            else if (input.Password == null)
            { throw new Exception("請輸入密碼"); }
            else if (input.Email == null)
            {
                throw new Exception("請輸入信箱");
            }
            else if (_userRepository.Any(x => x.EmailAddress == input.Email)) {
                throw new Exception("信箱重複");
            }
            else
            {
                var password = Hash256.GetSHA256(input.Password);

                User user = new User
                {
                    NickName = input.Account,
                    UserImg = "https://hackmd.io/_uploads/HyLghKZhR.png",
                    Account = input.Account,
                    WalletAmount = 0,
                    Country = "台灣",
                    City = "台北市",
                    Overview = "沒有輸入任何資料",
                    EmailAddress = input.Email,
                    State = 0,
                    Online = 1,
                    Password = password,
                    IsLocked = 1,
                };
                await _userRepository.AddAsync(user);
            }
        }

        public async Task<int> Get_UserId(RegiterViewModel revm)
        {

            var result = await _userRepository.FirstOrDefaultAsync(x => x.Account == revm.Account);


            return result.UserId;
        }



        public async Task SendMail_CheckEmail(int user)
        {
            var userData = await _userRepository.FirstOrDefaultAsync(x => x.UserId == user);
            var emailAddress = userData.EmailAddress;
            var ExpireTime = DateTime.UtcNow.AddMinutes(40);
            //編碼加密
            var verifyEmailDto = new VerifyEmailDto()
            {
                Email = emailAddress,
                ExpireTime = ExpireTime//15分鐘後到期
            };
            var serializeStr = Base64SerializerHelpers.SerializeInput(verifyEmailDto);
            var uri = _verifyEmailSettings.ReturnCheckEmailUrl;
            var returnUrl = QueryHelpers.AddQueryString(uri, "encodingParameter", serializeStr);
            var emailTemplate = EmailTemplateHelper.ChangePasswordVerifyEmail(userData.NickName, returnUrl);



            var email = new SendEmailRequest()
            {
                Receiver = userData.NickName,
                Body = emailTemplate,
                Subject = "Stellar帳戶信箱驗證來信",
                ReceiverEmail = userData.EmailAddress,


            };
            var verifyMailData = new VerifyMail()
            {
                UserId = user,
                EncodingParameter = serializeStr,
                Expired = ExpireTime
            };
            await _verifyMailRepository.AddAsync(verifyMailData);
            await _emailSenderService.SendEmailAsync(email);
        }



        public async Task<bool> VerifyEmail(string encodingParameter)
        {

            //解碼(encodingParameter被URLEncode過後了 框架會做)
            var verifyEmailDto = DeSerializeURLEncodeParameter<VerifyEmailDto>(encodingParameter);
            var verifyDataInDB = await _verifyMailRepository.FirstOrDefaultAsync(x => x.EncodingParameter == encodingParameter);
            var user = await _userRepository.FirstOrDefaultAsync(x => x.UserId == verifyDataInDB.UserId);
            var encodingParameterInDB = verifyDataInDB.EncodingParameter;
            var verifyEmailDtoInDB = DeSerializeURLEncodeParameter<VerifyEmailDto>(encodingParameterInDB);
            if (verifyEmailDto == null)
            {
                return false;
            }
            if (verifyEmailDtoInDB.Email != verifyEmailDto.Email)
            {
                return false;
            }
            //驗證是否過期
            if (verifyEmailDto.ExpireTime < DateTime.UtcNow)
            {
                return false;
            }

            //TODO驗證成功，更新資料庫
            try
            {
                user.State = 1;
                await _userRepository.UpdateAsync(user);

            }
            catch (Exception ex) { throw new Exception(ex.Message); }

            return true;
        }

        private T DeSerializeURLEncodeParameter<T>(string encodingParameter) where T : class
        {
            var decodeStr = HttpUtility.UrlDecode(encodingParameter);
            return Base64SerializerHelpers.DeSerializeParameter<T>(decodeStr);
        }




    }

}
