using ApplicationCore.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IPaymentQueryService
    {
       public Task<List<GetMakeOrderUseDataResult>> GetMakeOrderUseData(int userId);
       public Task<List<GetShoppingCartDataResult>> GetShoppingCartData(int userId);
    }
}
