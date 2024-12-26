using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
//using Infrastructure.Data.Mongo.Entity;

//using Infrastructure.Data.Mongo.Entity;
using System.Net.Mail;
using Web.Services.Authentication;
using Web.ViewModels.Member;


namespace Web.Services.Member
{
    public class AccountPageService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IChangePasswordService _changePasswordService;
        private readonly IRepository<VerifyMail> _verifyMailRepository;

        public AccountPageService(IRepository<User> userRepository, IChangePasswordService changePasswordService, IRepository<VerifyMail> verifyMailRepository)
        {
            _userRepository = userRepository;
            _changePasswordService = changePasswordService;
            _verifyMailRepository = verifyMailRepository;
        }

        //刪除帳號
        public async Task RemoveAccount(int userID)
        {
         var user = await _userRepository.FirstOrDefaultAsync(x => x.UserId == userID);
            // 檢查 user 是否為 null
            if (user == null)
            {
                // 處理找不到用戶的情況
                throw new Exception("User not found"); // 或者其他錯誤處理邏輯
            }
            user.IsLocked = 0;

            await _userRepository.UpdateAsync(user);
        }

        public async Task<AccountPageViewModel> GetAccountPageData(int Id)
        {
            int IdOfLoginUser = Id;            
            var user = await _userRepository.FirstOrDefaultAsync(x => x.UserId == IdOfLoginUser);


            var AccountPageData = new AccountPageViewModel
            {
                UserNickName = user.NickName,
                UserID = user.UserId,
                Country = user.Country,
                WalletAmount = user.WalletAmount,
                EmailAddress = user.EmailAddress,
                PhoneNumber = user.PhoneNumber,
                State = user.State,
            };
            return AccountPageData;
        }
    }
}
