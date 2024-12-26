using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Dtos
{
    public class GetShoppingCartDataResult
    {
        public int ProductId {  get; set; }
        public string? ProductMainImageUrl {  get; set; }
        public string? ProductName { get; set; }
        public int SalesPrice {  get; set; }
    }
}
