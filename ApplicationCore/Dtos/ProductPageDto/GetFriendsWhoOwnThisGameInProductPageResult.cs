using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Dtos.ProductPageDto
{

    public class GetFriendsWhoOwnThisGameInProductPageResult
    {
        public int Id { get; set; }
        public string ImgUrl { get; set; }
        public byte Online { get; set; }
        public string FriendName { get; set; }
    }
}
