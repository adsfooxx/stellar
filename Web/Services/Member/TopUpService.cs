using ApplicationCore.Interfaces;
using Web.ViewModels.Member;
namespace Web.Services.Member
{
    public class TopUpService
    {
        private readonly IRepository<User> _userRepository;

        public TopUpService(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<TopUpViewModel> GetTopUpAmount(int userId)
        {
            //var repository = new StellarRepository(new StellarDBContext());
            var topUpData = await _userRepository.FirstOrDefaultAsync((x) => x.UserId == userId);

            var topUp = new TopUpViewModel
            {
                UserId = userId,
                Account = topUpData.NickName,
                Balance = topUpData.WalletAmount,
                Amount = new List<Amount>
                {
                    new Amount { Funds = 150m },
                    new Amount { Funds = 300m },
                    new Amount { Funds = 750m },
                    new Amount { Funds = 1500m },
                    new Amount { Funds = 3000m },
                }
            };
            return topUp;
        }
    }
}