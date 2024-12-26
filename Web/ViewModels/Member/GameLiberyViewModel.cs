using Humanizer;

namespace Web.ViewModels.Member
{
    public class GameLiberyViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserImgUrl { get; set; }
        public int GameCount { get; set; }
        public List<GameViewModel> Games { get; set; }
    }

    public class GameViewModel {
        public int ProductId { get; set; }

        public string ProductName { get; set; }
        public string ProductImgUrl { get; set; }

        //public DateTime TotalDate { get; set; }
        public bool IsDownload { get; set; }
    }
}
