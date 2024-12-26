namespace Web.ViewModels.Partial
{
    public class StoreNavbarViewModel
    {
        public int UserId { get; set; }
        public int WishCount { get; set; }
        public int CartCount { get; set; }
        public List<Category> categories { get; set; }
        public List<Titles> Titles { get; set; }
    }
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
    public class Titles
    {
        public string Title { get; set; }
        public string TitleUrl { get; set; }
    }
}
