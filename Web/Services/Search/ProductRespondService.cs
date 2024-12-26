//using ApplicationCore.Interfaces;
//using Web.ViewModels.Product;
//using static Web.ViewModels.Product.ProductSearchViewModel;
//using Web.Controllers;
//using ApplicationCore.Entities;

//namespace Web.Services.Search
//{
//    public class ProductRespondServices
//    {
//        private readonly IRepository<Product> _productRepository;
//        private readonly IRepository<ProductComment> _productCommentRepository;

//        public ProductRespondServices(IRepository<Product> productRepository,
//IRepository<ProductComment> productCommentRepository)
//        {
//            _productRepository = productRepository;
//            _productCommentRepository = productCommentRepository;
//        }

//        public async Task<ProductSearchViewModel> GetProdctData()
//        {
//            從資料庫中異步檢索所有產品
//           var productsFromDb = await _productRepository.ListAsync();
//            var productComments = await _productCommentRepository.ListAsync();


//            初始化一個新的 ProductSearchViewModel 物件
//           var productData = new ProductSearchViewModel
//           {
//               products = new List<ProductSearchProduct>()
//           };

//            var query = from pc in productComments
//                        group pc by pc.ProductId into grouped
//                        select new
//                        {
//                            ProductId = grouped.Key,
//                            total_count = grouped.Count(),
//                            good = grouped.Count(x => x.Comment == true),
//                            good_in_percent = (decimal)grouped.Count(x => x.Comment == true) / grouped.Count()
//                        };


//            遍歷從資料庫獲得的產品清單，將每個產品轉換為 ProductSearchProduct 並添加到 productData 中
//            foreach (var product in productsFromDb)
//            {
//                decimal? goodPercentage = null;
//                foreach (var productComment in query)
//                {
//                    if (productComment.ProductId == product.ProductId)
//                    {
//                        goodPercentage = productComment.good_in_percent;
//                    }
//                }

//                productData.products.Add(new ProductSearchProduct
//                {
//                    ProductName = product.ProductName,
//                    ProductImg = product.ProductMainImageUrl,
//                    CommentImg = "https://localhost:7196/img/Product/Search/Good.png",
//                    CommentType = CalculateCommentType(goodPercentage),
//                    Startdate = product.ProductShelfTime.ToString("yyyy/MM/dd"), // 格式化日期
//                    DiscountPercent = product.Discount == 1 ? null : "-" + Convert.ToInt32((1 - product.Discount) * 100) + "%",
//                    OriginalPrice = product.ProductPrice,
//                    SalesPrice = (int)(product.ProductPrice = product.ProductPrice * product.Discount)

//                });
//            }

//            return productData;
//        }

//        public int CalculateCommentType(decimal? goodPercentage)
//        {
//            if (goodPercentage == null)
//            {
//                return 4; // Return 4 if goodPercentage is null
//            }

//            Convert nullable decimal to double for comparison

//           double goodPercentValue = (double)goodPercentage.Value;

//            if (goodPercentValue >= 0.6)
//                {
//                    return 1;
//                }

//            if (goodPercentValue >= 0.4)
//            {
//                return 2;
//            }

//            return 3;
//        }

//    }

//}

//namespace Web.Services.Search
//{

//    public class ProductRespondServices
//    {

//        public async Task<ProductSearchViewModel> GetProdctData()
//        {
//            var productdata = new ProductSearchViewModel
//            {
//                products = new List<ProductSearchProduct>
//            {
//                new ProductSearchProduct{ ProductName = "Counter-Strike 2",
//                    ProductImg ="https://localhost:7196/img/Product/Search/DIABLO.jpg",
//                    OperatingSystemImg = "https://localhost:7196/img/Product/Search/Windows.png",
//                    CommentImg = "https://localhost:7196/img/Product/Search/Good.png",
//                    Startdate = "2022/08/08",
//                    DiscountPercent = "20%",
//                    OriginalPrice = "NT$500",
//                    SalesPrice = 0},

//                    new ProductSearchProduct{
//                    ProductName = "鳴鳳絕殺_殺無生",
//                    ProductImg ="https://localhost:7196/img/Product/Search/DIABLO.jpg",
//                    OperatingSystemImg = "https://localhost:7196/img/Product/Search/Windows.png",
//                    CommentImg = "https://localhost:7196/img/Product/Search/Good.png",
//                    Startdate = "2022/08/08",
//                    DiscountPercent = "10%",
//                    OriginalPrice = "NT$500",
//                    SalesPrice = 1000},
//                    new ProductSearchProduct{ ProductName = "鳴鳳絕殺_殺無生",
//                    ProductImg ="https://localhost:7196/img/Product/Search/DIABLO.jpg",
//                    OperatingSystemImg = "https://localhost:7196/img/Product/Search/Windows.png",
//                    CommentImg = "https://localhost:7196/img/Product/Search/Good.png",
//                    Startdate = "2022/08/08",
//                    DiscountPercent = "50%",
//                    OriginalPrice = "NT$500",
//                    SalesPrice = 0},
//                    new ProductSearchProduct{ ProductName = "鳴鳳絕殺_殺無生",
//                    ProductImg ="https://localhost:7196/img/Product/Search/DIABLO.jpg",
//                    OperatingSystemImg = "https://localhost:7196/img/Product/Search/Windows.png",
//                    CommentImg = "https://localhost:7196/img/Product/Search/Good.png",
//                    Startdate = "2022/08/08",
//                    DiscountPercent = "50%",
//                   OriginalPrice= "NT$500",
//                    SalesPrice = 500},
//                    new ProductSearchProduct{ ProductName = "鳴鳳絕殺_殺無生",
//                     ProductImg ="https://localhost:7196/img/Product/Search/DIABLO.jpg",
//                    OperatingSystemImg = "https://localhost:7196/img/Product/Search/Windows.png",
//                    CommentImg = "https://localhost:7196/img/Product/Search/Good.png",
//                    Startdate = "2022/08/08",
//                    DiscountPercent = "50%",
//                   OriginalPrice = "NT$500",
//                    SalesPrice = 0},
//                    new ProductSearchProduct{ ProductName = "鳴鳳絕殺_殺無生",
//                     ProductImg ="https://localhost:7196/img/Product/Search/DIABLO.jpg",
//                    OperatingSystemImg = "https://localhost:7196/img/Product/Search/Windows.png",
//                    CommentImg = "https://localhost:7196/img/Product/Search/Good.png",
//                    Startdate = "2022/08/08",
//                    DiscountPercent = "50%",
//                   OriginalPrice= "NT$500",
//                    SalesPrice = 300},
//             }
//            };
//            return productdata;
//        }
//    }
//}



