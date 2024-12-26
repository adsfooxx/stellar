using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Dtos.CustpmerSupportChatBot
{
    public class OrderResult
    {
        public int UserId { get; set; }
        public int OrderId { get; set; }
        public DateTime Orderdate { get; set; }
        public string ProductName { get; set; }
        public decimal SalesPrice { get; set; }
        public string TransactionType { get; set; }
    }
}
