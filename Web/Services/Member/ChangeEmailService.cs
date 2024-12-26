using ApplicationCore.Dtos.Mail;
using ApplicationCore.Interfaces;
using Infrastructure.Data.Mail;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Mail;
using System.Web;
using Web.Helpers;
using Web.ViewModels.Member;

namespace Web.Services.Member
{
    public class ChangeEmailService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<VerifyMail> _verifyMailRepository;
        private readonly IEmailSenderService _emailSenderService;
        private VerifyEmailSettings _verifyEmailSettings;

        public ChangeEmailService(IRepository<User> userRepository, IRepository<VerifyMail> verifyMailRepository, IEmailSenderService emailSenderService, VerifyEmailSettings verifyEmailSettings)
        {
            _userRepository = userRepository;
            _verifyMailRepository = verifyMailRepository;
            _emailSenderService = emailSenderService;
            _verifyEmailSettings = verifyEmailSettings;
        }


        public async Task<string> UpdateEmailAddress(int userID, string emailAddress)
        {
            var user = await _userRepository.FirstOrDefaultAsync(x => x.UserId == userID);


            // 檢查是否有其他使用者使用相同的電子郵件地址
            var isEmailExist = (from u in await _userRepository.ListAsync()
                                where u.EmailAddress == emailAddress
                                select u).Any();

            if (isEmailExist)
            {
                // 電子郵件已存在，返回錯誤訊息
                return "The email address is already in use.";
            }
            else
            {
                // 更新電子郵件
                user.EmailAddress = emailAddress;
                user.State = 0;
                await _userRepository.UpdateAsync(user);
                return "Email address updated successfully.";
            }
        }


        public async Task<ChangeEmailViewModel> GetChangeEmailData(int Id)
        {
            int IdOfLoginUser = Id;

            var user = await _userRepository.FirstOrDefaultAsync(x => x.UserId == IdOfLoginUser);

            var GetChangeEmailData = new ChangeEmailViewModel
            {
                UserNickName = user.NickName,
                UserID = user.UserId,
                EmailAddress = user.EmailAddress,
            };

            return GetChangeEmailData;
        }


        //-----------------------------------------------------------------------------------------------------------------------------------------------------
        //驗證


        public async Task SendCheckEmail(int user)
        {
            try
            {
                var userData = await _userRepository.FirstOrDefaultAsync(x => x.UserId == user);
                var emailAddress = userData.EmailAddress;
                var ExpireTime = DateTime.UtcNow.AddMinutes(10);

                var verifyEmailDto = new VerifyEmailDto()
                {
                    Email = emailAddress,
                    ExpireTime = ExpireTime
                };

                var serializeStr = Base64SerializerHelpers.SerializeInput(verifyEmailDto);

                // 使用 ReturnCheckEmailUrl 來生成驗證連結
                var uri = _verifyEmailSettings.ReturnCheckChangeEmailUrl;
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
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send verification email: {ex.Message}");
                throw;
            }
        }




        public async Task<bool> VerifyCheckEmail(string encodingParameter)
        {
            try
            {
                // 解碼參數並反序列化
                var verifyEmailDto = DeSerializeURLEncodeParameter<VerifyEmailDto>(encodingParameter);
                if (verifyEmailDto == null)
                {
                    return false;
                }

                // 從資料庫查詢驗證記錄
                var verifyDataInDB = await _verifyMailRepository.FirstOrDefaultAsync(x => x.EncodingParameter == encodingParameter);
                if (verifyDataInDB == null)
                {
                    return false;
                }

                // 獲取用戶資料
                var user = await _userRepository.FirstOrDefaultAsync(x => x.UserId == verifyDataInDB.UserId);
                if (user == null)
                {
                    return false;
                }

                // 驗證參數是否匹配
                var verifyEmailDtoInDB = DeSerializeURLEncodeParameter<VerifyEmailDto>(verifyDataInDB.EncodingParameter);
                if (verifyEmailDtoInDB.Email != verifyEmailDto.Email)
                {
                    return false;
                }

                // 檢查驗證連結是否過期
                if (verifyEmailDto.ExpireTime < DateTime.UtcNow)
                {
                    return false;
                }

                // 驗證通過，更新用戶狀態
                user.State = 1; // 假設 1 表示已認證
                await _userRepository.UpdateAsync(user);

                return true;
            }
            catch (Exception ex)
            {
                // 記錄錯誤信息並返回 false
                Console.WriteLine($"Error during email verification: {ex.Message}");
                return false;
            }
        }


        private T DeSerializeURLEncodeParameter<T>(string encodingParameter) where T : class
        {
            var decodeStr = HttpUtility.UrlDecode(encodingParameter);
            return Base64SerializerHelpers.DeSerializeParameter<T>(decodeStr);
        }







    }
}
