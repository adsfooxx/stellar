using ApplicationCore.Dtos.Authentication;
using ApplicationCore.Dtos.Mail;

using ApplicationCore.Interfaces;
using Infrastructure.Data.Mail;
using Infrastructure.Data.MailKit;
using Infrastructure.Services.MailKit;
using Microsoft.AspNetCore.WebUtilities;
using System.Web;
using Web.Helpers;

namespace Web.Services.Authentication
{
    public interface IChangePasswordService
    {
        Task<bool> VerifyEmail(string encodingParameter);
    }

    public class ChangePasswordService : IChangePasswordService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IEmailSenderService _emailSenderService;
        private VerifyEmailSettings _verifyEmailSettings;
        private readonly IRepository<VerifyMail> _verifyMailRepository;

        public ChangePasswordService(IRepository<User> userRepository, IEmailSenderService emailSenderService, VerifyEmailSettings verifyEmailSettings, IRepository<VerifyMail> verifyMail)
        {
            _userRepository = userRepository;
            _emailSenderService = emailSenderService;
            _verifyEmailSettings = verifyEmailSettings;
            _verifyMailRepository = verifyMail;
        }

        public async Task SendVerifyMail(int user)
        {
            var userData = await _userRepository.FirstOrDefaultAsync(x => x.UserId == user);
            var emailAddress = userData.EmailAddress;
            var ExpireTime = DateTime.UtcNow.AddMinutes(15);
            //編碼加密
            var verifyEmailDto = new VerifyEmailDto()
            {
                Email = emailAddress,
                ExpireTime = ExpireTime//15分鐘後到期
            };
            var serializeStr= Base64SerializerHelpers
                .SerializeInput(verifyEmailDto);
            var uri = _verifyEmailSettings.ReturnUrl;
            var returnUrl = QueryHelpers
                .AddQueryString(uri, "encodingParameter", serializeStr);
            var emailTemplate = EmailTemplateHelper
                .ChangePasswordVerifyEmail(userData.NickName, returnUrl);



            var email = new SendEmailRequest()
            {
                Receiver = userData.NickName,
                Body= emailTemplate,
                Subject="Stellar帳戶驗證來信",
                ReceiverEmail= userData.EmailAddress,

                
            };
            var verifyMailData = new VerifyMail()
            {
                UserId= user,
                EncodingParameter= serializeStr,
                Expired= ExpireTime
            };
            await _verifyMailRepository.AddAsync(verifyMailData);
            await _emailSenderService.SendEmailAsync(email);
        }

        public async Task<bool> VerifyEmail(string encodingParameter)
        {
            
            //解碼(encodingParameter被URLEncode過後了 框架會做)
            var verifyEmailDto = DeSerializeURLEncodeParameter<VerifyEmailDto>(encodingParameter);
            var verifyDataInDB = await _verifyMailRepository.FirstOrDefaultAsync(x=>x.EncodingParameter== encodingParameter);
            var encodingParameterInDB= verifyDataInDB.EncodingParameter;
            var verifyEmailDtoInDB= DeSerializeURLEncodeParameter<VerifyEmailDto>(encodingParameterInDB);
            if (verifyEmailDto == null)
            {
                return false;
            }
            if(verifyEmailDtoInDB.Email != verifyEmailDto.Email)
            {
                return false;
            }
            //驗證是否過期
            if (verifyEmailDto.ExpireTime < DateTime.UtcNow)
            {
                return false;
            }


            return true;
        }

        private T DeSerializeURLEncodeParameter<T>(string encodingParameter) where T : class
        {
            var decodeStr = HttpUtility.UrlDecode(encodingParameter);
            return Base64SerializerHelpers.DeSerializeParameter<T>(decodeStr);
        }


        public async Task<string> ResetPassword(int userID, ChangePasswordDto changeData ) 
        {
            var userdata = await _userRepository.FirstOrDefaultAsync((x) => x.UserId == userID);
            var changeDataOldPwd = Hash256.GetSHA256(changeData.CurrentPassword);
            if(userdata.Password == changeDataOldPwd)
            {
                userdata.Password = Hash256.GetSHA256(changeData.NewPassword);
                await _userRepository.UpdateAsync(userdata);
                return "變更成功";
            }
            else
            {
                return "原密碼不正確";
            }

        }

    }
}
