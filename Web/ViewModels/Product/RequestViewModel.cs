namespace Web.ViewModels.Product
{
    public class RequestViewModel
    {

        public string InputValue { get; set; }

        public string OrderSelect { get; set; }
        public List<PriceSelect> PriceSelect { get; set; }
        public List<ProductTypeSelect> ProductTypeSelect { get; set; }

        public int PricerRange { get; set; }

        public List<ProductConnectSelect> ProductConnectSelect { get; set; }

    }
    public class PriceSelect
    {         //價格種類

        public string PriceType { get; set; }

    }
    public class ProductTypeSelect
    {   //產品種類
        public string ProductType { get; set; }
    }


    public class ProductConnectSelect  //連接種類
    {
        public string ConnectType { get; set; }
    }


}

