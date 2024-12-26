using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Dtos
{
    public class GetProductInProductSupportResult
    {
        public int UserId { get; set; }
        public string Account { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductMainImageUrl { get; set; }
        public string OrderDate { get; set; }
    }
}
