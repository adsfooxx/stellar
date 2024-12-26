using ApplicationCore.Interfaces;
using Web.Models;
using Web.ViewModels.CustomerSupport;
using Web.ViewModels.Home;
using Web.ViewModels.Member;
using Web.ViewModels.Product;

namespace Web.Services.Home
{
    public class HomeService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductCarousel> _productcarouselRepository;
        private readonly IRepository<TagConnect> _tagConnectRepository;
        private readonly IRepository<ApplicationCore.Entities.Tag> _tagRepository;
        private readonly IRepository<ProductComment> _productCommentRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<ProductsDiscount> _discountRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<ProductCollection> _productCollectionRepository;

        public HomeService(IRepository<Product> productRepository, IRepository<ProductCarousel> productcarouselRepository, IRepository<TagConnect> tagConnectRepository, IRepository<ApplicationCore.Entities.Tag> tagRepository, IRepository<ProductComment> productCommentRepository, IRepository<Category> categoryRepository, IRepository<ProductsDiscount> discountRepository, IRepository<User> userRepository, IRepository<ProductCollection> productCollectionRepository)
        {
            _productRepository = productRepository;
            _productcarouselRepository = productcarouselRepository;
            _tagConnectRepository = tagConnectRepository;
            _tagRepository = tagRepository;
            _productCommentRepository = productCommentRepository;
            _categoryRepository = categoryRepository;
            _discountRepository = discountRepository;
            _userRepository = userRepository;
            _productCollectionRepository = productCollectionRepository;
        }
        public async Task<HomeVMBy7N> GetHomeServiceData(int loginUserId)
        {
            //int indexProduct = 40;
            var random = new Random();
            bool login = false;
            if (loginUserId >= 0) { login = true; } else { login = false; }
            var oneYearAgo = DateOnly.FromDateTime(DateTime.Now.AddYears(-1)); // 一年前的日期
            var futureDate = DateOnly.FromDateTime(DateTime.Now); // 当前时间




            var allSupImg = (from P in await _productRepository.ListAsync(x => (x.ProductShelfTime >= oneYearAgo || x.ProductShelfTime > futureDate) && x.ProductStatus == 1)
                             join PC in await _productcarouselRepository.ListAsync(x => !x.DataSourceUrl.Contains("video"))
                             on P.ProductId equals PC.ProductId
                             select new SupImg
                             {
                                 ProductId = P.ProductId,
                                 SupImgUrl = PC.CarouselUrl
                             }
                            ).ToList();
            var allTag = (from P in await _productRepository.ListAsync(x => (x.ProductShelfTime >= oneYearAgo || x.ProductShelfTime > futureDate) && x.ProductStatus == 1)
                          join TC in await _tagConnectRepository.ListAsync()
                          on P.ProductId equals TC.ProductId
                          join T in await _tagRepository.ListAsync()
                          on TC.TagId equals T.TagId
                          select new ViewModels.Home.Tag
                          {
                              ProductId = P.ProductId,
                              TagName = T.TagName
                          }).ToList();
            var tPList = (from p in await _productRepository.ListAsync(x => (x.ProductShelfTime >= oneYearAgo || x.ProductShelfTime > futureDate) && x.ProductStatus == 1)
                          join D in await _discountRepository.ListAsync()
                         on p.ProductId equals D.ProductId into discountGroup
                          from discount in discountGroup.DefaultIfEmpty()
                          select new TopProduct
                          {
                              ProductId = p.ProductId,
                              ProductName = p.ProductName,
                              MainImg = p.ProductMainImageUrl,
                              Price = p.ProductPrice,
                              Discount = discount?.Discount ?? 1,
                              SupImg = null!,
                              TagList = null!,
                              CraftTime = p.ProductShelfTime,
                              SalesPrice = p.ProductPrice * (discount?.Discount ?? 1)
                          }
                         ).OrderByDescending(x => x.CraftTime).OrderBy(_ => random.Next()).Take(10).ToList();
            foreach (var tp in tPList)
            {
                tp.SupImg = allSupImg.Where(x => x.ProductId == tp.ProductId).Take(4).ToList();
                tp.TagList = allTag.Where(x => x.ProductId == tp.ProductId).ToList();

            }
            //特價時間要改  V
            var sP = (from p in await _productRepository.ListAsync(x => (x.ProductShelfTime >= oneYearAgo || x.ProductShelfTime > futureDate) && x.ProductStatus == 1)
                      join D in await _discountRepository.ListAsync(x => x.SalesStartDate <= DateOnly.FromDateTime(DateTime.Now) && x.SalesEndDate > DateOnly.FromDateTime(DateTime.Now))
on p.ProductId equals D.ProductId
                      orderby D.Discount
                      select new SpecialProduct
                      {
                          Id = p.ProductId,
                          ProductName = p.ProductName,
                          MainImg = p.ProductMainImageUrl,
                          Price = p.ProductPrice,
                          Discount = D.Discount,
                          SalesPrice = p.ProductPrice * D.Discount,
                          OverTime = D.SalesEndDate
                      }

                     ).OrderBy(_ => random.Next()).Take(12).ToList();
            var pL = (from P in await _productRepository.ListAsync(x => (x.ProductShelfTime >= oneYearAgo || x.ProductShelfTime > futureDate) && x.ProductStatus == 1)
                      join D in await _discountRepository.ListAsync(x => x.SalesStartDate <= DateOnly.FromDateTime(DateTime.Now) && x.SalesEndDate > DateOnly.FromDateTime(DateTime.Now))
                         on P.ProductId equals D.ProductId into discountGroup
                      from discount in discountGroup.DefaultIfEmpty()
                      select new ProductList
                      {
                          ProductId = P.ProductId,
                          ProductName = P.ProductName,
                          MainImg = P.ProductMainImageUrl,
                          Price = P.ProductPrice,
                          Discount = discount?.Discount ?? 1,
                          SalesPrice = P.ProductPrice * (discount?.Discount ?? 1),
                          TagList = null!,
                          CommendString = null!,
                          ProductCommend = 0,
                          SupImg = null!,
                          HasGameNum = 0,
                      }
                    ).OrderBy(_ => random.Next()).ToList();






            foreach (var tp in pL)
            {
                tp.HasGameNum = (await _productCollectionRepository.ListAsync(x => x.ProductId == tp.ProductId)).Count();
                tp.SupImg = allSupImg.Where(x => x.ProductId == tp.ProductId).Take(5).ToList();
                tp.TagList = allTag.Where(x => x.ProductId == tp.ProductId).ToList();
                tp.ProductCommend = (await _productCommentRepository.ListAsync(x => x.ProductId == tp.ProductId)).Count();

                double x = 0;
                double y = 0;
                double num = 0;

                if (tp.ProductCommend != 0)
                {
                    x = (await _productCommentRepository.ListAsync(x => x.ProductId == tp.ProductId && x.Comment == true)).Count();
                    y = tp.ProductCommend;
                    num = x / y;
                }
                if (tp.ProductCommend == 0) { tp.CommendString = "無人評價"; }
                else if (num < 0.4) { tp.CommendString = "大多負評"; }
                else if (num < 0.6) { tp.CommendString = "褒貶不一"; }
                else if (num >= 0.6) { tp.CommendString = "大多好評"; }
            }


            var categorySp = (from p in await _categoryRepository.ListAsync()
                              select new CategorySpList
                              {
                                  CategoryID = p.CategoryId,
                                  CategoryName = p.CategoryName,
                                  CategoryImgUrl = p.CategoryImgUrl,

                              }).Take(16).ToList();


            var reVM = new HomeVMBy7N()
            {
                topProducts = tPList,
                SpecialProduct = sP,
                ProductList = pL,
                //HoverView = hV,
                CategorySpLists = categorySp,
                login = login,
            };



            return reVM;
        }

    }
}
