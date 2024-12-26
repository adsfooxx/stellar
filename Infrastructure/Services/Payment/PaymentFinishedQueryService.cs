using ApplicationCore.Dtos;
using ApplicationCore.Interfaces;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Payment
{
    public class PaymentFinishedQueryService : IPaymentQueryService
    {
        private readonly string _connectionString;
        private readonly IDbConnection _dbConnection;

        public PaymentFinishedQueryService(IConfiguration configuration, IDbConnection dbConnection)
        {
            _connectionString = configuration.GetConnectionString("StellarDB") ??
                throw new ArgumentNullException("找不到連線字串");
            _dbConnection = dbConnection;
        }
        public async Task<List<GetMakeOrderUseDataResult>> GetMakeOrderUseData(int userId)
        {
            var sql = @"SELECT
                            p.ProductId,
                            p.ProductMainImageUrl,
                            p.ProductName,
                            p.ProductPrice * IIF(pd.Discount IS NOT NULL AND GETDATE() >= pd.SalesStartDate AND GETDATE() < pd.SalesEndDate, pd.Discount, 1) AS SalesPrice
                        FROM Product AS p
                        INNER JOIN ShoppingCartCard AS sc ON sc.ProductId = p.ProductId
                        INNER JOIN Users AS u ON sc.UserId = u.UserId
                        LEFT JOIN ProductsDiscount AS pd ON p.ProductId = pd.ProductId AND (pd.SalesEndDate IS NULL OR GETDATE() <= pd.SalesEndDate)
                        WHERE u.UserId = @UserId
                        AND p.ProductPrice > 0;";
            var result = await _dbConnection.QueryAsync<GetMakeOrderUseDataResult>(sql, new { UserId = userId });
            var resultList = result.ToList();
            return resultList;
        }

        public async Task<List<GetShoppingCartDataResult>> GetShoppingCartData(int userId)
        {
            var sql = @"SELECT
                            p.ProductId,
                            p.ProductMainImageUrl,
                            p.ProductName,
                            ROUND(p.ProductPrice * IIF(pd.Discount IS NOT NULL AND GETDATE() >= pd.SalesStartDate AND GETDATE() < pd.SalesEndDate, pd.Discount, 1),0) AS SalesPrice
                        FROM Product AS p
                        INNER JOIN ShoppingCartCard AS sc ON sc.ProductId = p.ProductId
                        INNER JOIN Users AS u ON sc.UserId = u.UserId
                        LEFT JOIN ProductsDiscount AS pd ON p.ProductId = pd.ProductId AND (pd.SalesEndDate IS NULL OR GETDATE() <= pd.SalesEndDate)
                        WHERE u.UserId = @UserId
                        AND p.ProductPrice > 0;";
            var result = await _dbConnection.QueryAsync<GetShoppingCartDataResult>(sql, new { UserId = userId });
            var resultList = result.ToList();
            return resultList;
        }
    }
}
