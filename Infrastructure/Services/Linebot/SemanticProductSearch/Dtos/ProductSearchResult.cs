using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Linebot.SemanticProductSearch.Dtos
{
    public class ProductSearchResult
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string Relevance { get; set; }
    }
}
