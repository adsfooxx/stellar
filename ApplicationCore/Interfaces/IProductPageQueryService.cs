using ApplicationCore.Dtos.ProductPageDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IProductPageQueryService
    {
        Task<List<GetFriendsWhoOwnThisGameInProductPageResult>> GetFriendsWhoOwnThisGameInProductPage(int currentProductId, int currentUserId);
        Task<List<GetTagsInProductPageResult>> GetTagsInProductPage(int currentProductId);
    }
}
