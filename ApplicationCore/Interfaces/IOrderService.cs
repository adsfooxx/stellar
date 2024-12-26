using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IOrderService
    {                                 //參數以實作需要再加以定義
        Task<int> CreateOrderAsync(int usetId,List<PurchaseHistoryDetail> purchaseHistoryDetails);
    }
}
