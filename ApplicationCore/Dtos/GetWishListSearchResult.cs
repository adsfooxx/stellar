using ApplicationCore.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Dtos
{
    public interface IWishListSearchQueryService
    {
        Task<List<GetWishListSearchResult>> SearchWishItemsForUser(int userId, string searchTerm);
    }
    public class GetWishListSearchResult
    {
        public List<WishCardSearch> WishListItemSearch { get; set; }
    }

    public class WishCardSearch
    {
        public int SortID { get; set; }
        public string Name { get; set; }
        public int ProductId { get; set; }
    }
}
