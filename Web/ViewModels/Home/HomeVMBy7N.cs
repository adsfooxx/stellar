using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace Web.ViewModels.Home
{
    public class HomeVMBy7N
    {
        public List<TopProduct> topProducts { get; set; }
        public List<SpecialProduct> SpecialProduct { get; set; }

        public List<ProductList> ProductList { get; set; }
        //public List<HoverView> HoverView { get; set; }
        public List<CategorySpList> CategorySpLists { get; set; }
        public bool login {  get; set; }
    }

    public class TopProduct
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string MainImg { get; set; }
        public List<SupImg> SupImg { get; set; }

        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal SalesPrice { get; set; }
        public List<Tag> TagList { get; set; }
        public DateOnly CraftTime { get; set; }

    }

    public class SupImg
    {
        public int ProductId { get; set; }
        public string SupImgUrl { get; set; }

    }
    public class Tag
    {
        public int ProductId { get; set; }
        public string TagName { get; set; }
    }

    public class SpecialProduct
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string MainImg { get; set; }
        //特價到期時間
        public DateOnly OverTime { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal SalesPrice { get; set; }
    }
    public class ProductList
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string MainImg { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal SalesPrice { get; set; }
        public List<Tag> TagList { get; set; }
        public List<SupImg> SupImg { get; set; }
        public int ProductCommend { get; set; }
        public string CommendString { get; set; }
        public int HasGameNum { get; set; }

    }
    //public class HoverView
    //{
    //    public int ProductId { get; set; }
    //    public string ProductName { get; set; }
    //    public List<Tag> TagList { get; set; }


    //}

    public class CategorySpList
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string CategoryImgUrl { get; set; }

    }
}
