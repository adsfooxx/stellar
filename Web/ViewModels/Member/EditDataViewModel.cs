using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels.Member
{
    public class EditDataViewModel
    {
        public List<SelectListItem> Countries { get; } = new List<SelectListItem>()
        {
            new SelectListItem{Text="台灣",Value="台灣"},
        };
        public List<SelectListItem> Cities { get; } = new List<SelectListItem>()
        {
            new SelectListItem{Text="台北市",Value="台北市"},
            new SelectListItem{Text="新北市",Value="新北市"},
            new SelectListItem{Text="基隆市",Value="基隆市"},
            new SelectListItem{Text="桃園市",Value="桃園市"},
            new SelectListItem{Text="新竹縣",Value="新竹縣"},
            new SelectListItem{Text="新竹市",Value="新竹市"},
            new SelectListItem{Text="宜蘭縣",Value="宜蘭縣"},
            new SelectListItem{Text="台中市",Value="台中市"},
            new SelectListItem{Text="苗栗縣",Value="苗栗縣"},
            new SelectListItem{Text="彰化縣",Value="彰化縣"},
            new SelectListItem{Text="南投縣",Value="南投縣"},
            new SelectListItem{Text="雲林縣",Value="雲林縣"},
            new SelectListItem{Text="台南市",Value="台南市"},
            new SelectListItem{Text="高雄市",Value="高雄市"},
            new SelectListItem{Text="嘉義縣",Value="嘉義縣"},
            new SelectListItem{Text="嘉義市",Value="嘉義市"},
            new SelectListItem{Text="屏東縣",Value="屏東縣"},
            new SelectListItem{Text="花蓮縣",Value="花蓮縣"},
            new SelectListItem{Text="台東縣",Value="台東縣"},
            new SelectListItem{Text="澎湖縣",Value="澎湖縣"},
            new SelectListItem{Text="金門縣",Value="金門縣"},
            new SelectListItem{Text="連江縣（馬祖）",Value="連江縣（馬祖）"},
        };

        public int UserId {  get; set; }

        [Required(ErrorMessage ="請輸入暱稱")]
        public string NickName { get; set; }
        public string UserName {  get; set; }
        public string ImgUrl {  get; set; }
        public IFormFile File { get; set; }
        public string Country {  get; set; }
        public string City { get; set; }
        //概述
        public string OverView {  get; set; }
    }
}
