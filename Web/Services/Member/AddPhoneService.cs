using ApplicationCore.Interfaces;
using Web.ViewModels.Member;
using ApplicationCore.Dtos.AccountPageDto;
using ApplicationCore.Dtos.Authentication;
using Web.Helpers;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Entities;

namespace Web.Services.Member
{
    public class AddPhoneService
    {

        private readonly IRepository<User> _userRepository;
        public AddPhoneService(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }


        public async Task UpdatePhoneNumber(int userID, string phonenumber)
        {



            var user = await _userRepository.FirstOrDefaultAsync(x => x.UserId == userID);

            user.PhoneNumber = phonenumber;

            await _userRepository.UpdateAsync(user);
        }


        public async Task<AddPhoneViewModel> GetAddPhoneData(int Id)
        {
            int IdOfLoginUser = Id;

            var user = await _userRepository.FirstOrDefaultAsync(x => x.UserId == IdOfLoginUser);

            var AddPhoneData = new AddPhoneViewModel
            {
                UserNickName = user.NickName,
                UserID = user.UserId,
                PhoneNumber = user.PhoneNumber,
            };
            return AddPhoneData;
        }

    }
}
