using ApplicationCore.Interfaces;
using Infrastructure.Services;
using Web.ViewModels.Member;

namespace Web.Services.Member
{
    public class EditDataService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<HistoryName> _historyNameRepository;
        private readonly CloudinaryService _cloudinaryService;
        public EditDataService(IRepository<User> userRepository, IRepository<HistoryName> historyNameRepository, CloudinaryService cloudinaryService)
        {
            _userRepository = userRepository;
            _historyNameRepository = historyNameRepository;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<EditDataViewModel> GetUserData(int userId)
        {

            var userData = await  _userRepository.GetByIdAsync(userId);
            if(userData is null)
            {
                return new EditDataViewModel()
                {
                    UserId = -1
                };
            }
            var userDataToView = new EditDataViewModel
            {
                UserId = userData.UserId,
                NickName = userData.NickName,
                UserName = userData.UserName!,
                ImgUrl = userData.UserImg!,
                Country = userData.Country!,
                City = userData.City!,
                OverView = userData.Overview!,
            };
            var selectCountry= userDataToView.Countries.FirstOrDefault(x => x.Text == userData.Country)?.Selected;
            var selectCity = userDataToView.Cities.FirstOrDefault(y => y.Text == userData.City)?.Selected;
            if(selectCountry is not null || selectCity is not null) { 
            selectCountry = true;
            selectCity = true;
            }
            return userDataToView;

        }
        public async Task  EditUserData(EditDataViewModel user)
        {
            var userDataInDB = await _userRepository.GetByIdAsync(user.UserId);
            var now = DateTime.Now;
            var imgUrl = _cloudinaryService.UploadImage(user.File) ?? user.ImgUrl;
            var historyNames = await _historyNameRepository.ListAsync(x => x.UserId == user.UserId);

            if (userDataInDB.NickName != user.NickName && !(historyNames.Any(y => y.OldName == user.NickName)))
            {
                var historyname = new HistoryName()
                {
                    UserId = user.UserId,
                    OldName = userDataInDB.NickName,
                    Time = DateOnly.FromDateTime(DateTime.Now)
                };
               await _historyNameRepository.AddAsync(historyname);
            }


            userDataInDB.NickName = user.NickName;
            userDataInDB.UserName = user.UserName;
            userDataInDB.UserImg = imgUrl;
            userDataInDB.Country = user.Country;
            userDataInDB.City = user.City;
            userDataInDB.Overview = user.OverView;
            await _userRepository.UpdateAsync(userDataInDB);
        }
    }
}
