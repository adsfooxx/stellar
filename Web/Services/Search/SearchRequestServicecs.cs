
using Microsoft.AspNetCore.Mvc.Diagnostics;
using System.IO;
using Web.ViewModels.Payment;
using Web.ViewModels.Product;
using static Web.ViewModels.Product.RequestViewModel;

namespace Web.Services.Search
{
    public class SearchRequestServices
    {

        public Task<RequestViewModel> GetSelectData()
        {
            var selectdata = new RequestViewModel              
                {
                    InputValue = "輸入搜尋詞彙或標籤",
                    OrderSelect = "相關性",
                    PriceSelect = new List<PriceSelect>
                {
                    new PriceSelect { PriceType = "特別優惠" },
                    new PriceSelect { PriceType = "隱藏免費遊玩" },
                },

                    ProductTypeSelect = new List<ProductTypeSelect>
                {
                    new ProductTypeSelect { ProductType = "單人" },
                    new ProductTypeSelect{ ProductType = "獨立" },
                    new ProductTypeSelect{ ProductType = "冒險" },
                    new ProductTypeSelect{ ProductType = "動作" },
                    new ProductTypeSelect{ ProductType = "2D" },
                    new ProductTypeSelect{ ProductType = "休閒" },
                    new ProductTypeSelect{ ProductType = "探索" },
                    new ProductTypeSelect{ ProductType = "解謎" },
                    new ProductTypeSelect{ ProductType = "角色扮演" },
                    new ProductTypeSelect{ ProductType = "劇情豐富" },
                },

                    ProductConnectSelect = new List<ProductConnectSelect>
                {
                   new ProductConnectSelect{ConnectType = "多人" },
                   new ProductConnectSelect{ConnectType = "單人" },
                   new ProductConnectSelect{ConnectType = "玩家對戰" },
                   new ProductConnectSelect{ConnectType = "線上玩家對戰" },
                   new ProductConnectSelect{ConnectType = "區域網路玩家對戰" },
                   new ProductConnectSelect{ConnectType = "共享/分割螢幕玩家對戰" },
                   new ProductConnectSelect{ConnectType = "合作" },
                   new ProductConnectSelect{ConnectType = "線上合作" },
                   new ProductConnectSelect{ConnectType = "區域網路合作" },
                   new ProductConnectSelect{ConnectType = "跨平台多人連線" },
                
                }
            };
            return Task.FromResult(selectdata);

        }
    }
}




