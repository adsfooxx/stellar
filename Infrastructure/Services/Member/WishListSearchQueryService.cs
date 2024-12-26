using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Dtos;
using ApplicationCore.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace Infrastructure.Services.Member
{
    class WishListSearchQueryService
    {
        private readonly string _connectionString;
        private readonly IDbConnection _dbConnection;

        public WishListSearchQueryService(IConfiguration configuration, IDbConnection dbConnection)
        {
            _connectionString = configuration.GetConnectionString("StellarDB") ??
                throw new ArgumentNullException("找不到連線字串");
            _dbConnection = dbConnection;
        }

        public async Task<GetProductInProductSupportResult> GetProductInProductSupport(int userId, int productId)
        {
            var sql = @"SELECT 
                        u.UserId,
                        u.Account,
                        p.ProductId,
                        p.ProductName,
                        p.ProductMainImageUrl,
                        o.Orderdate,
						o.State
						FROM Users AS u
						INNER JOIN ProductCollection AS pc
                        ON pc.UserId = u.UserId
						INNER JOIN Product AS p
                        ON p.ProductId = pc.ProductId
						LEFT JOIN [Order] AS o
						ON u.UserId = o.UserId
                        LEFT JOIN PurchaseHistoryDetail AS ph
                        ON o.OrderId =  ph.OrderId
                        WHERE u.UserId = @UserId AND pc.ProductId =@ProductId AND o.State = 1
						ORDER BY  o.Orderdate DESC";
            var result = await _dbConnection.QueryAsync<GetProductInProductSupportResult>(sql, new { UserId = userId, ProductId = productId });

            return result.First();
        }
    }
}
