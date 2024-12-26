using MongoDB.Driver;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Dapper;
using Infrastructure.Data.Mongo.Repository;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Infrastructure.Data.Mongo.Entity;
using ApplicationCore.Dtos.CustpmerSupportChatBot;
using MongoDB.Bson.IO;
using System.Text.Json;
namespace Infrastructure.Service.MongoDB
{
    [Experimental("SKEXP0001")]
    public class SemanticKernelSearchService 
    {
        private readonly MongoRepository<Products> _productInMongoRepository;
        private readonly string _connectionString;
        private readonly IDbConnection _dbConnection;
        private readonly IRepository<ApplicationCore.Entities.User> _userRepository;
        public SemanticKernelSearchService(MongoRepository<Products> productInMongoRepository, IConfiguration configuration, IDbConnection dbConnection)
        {
            _productInMongoRepository = productInMongoRepository;
            _connectionString = configuration.GetConnectionString("StellarDB") ??
               throw new ArgumentNullException("找不到連線字串");
            _dbConnection = dbConnection;
        }


        public async Task<IEnumerable<Products>> GetFetchData()
        {
            var sql = @"
                            SELECT 
                                p.ProductId,
                                p.ProductName,
                                p.ProductPrice,
                                p.ProductPrice * IIF(pd.Discount IS NOT NULL AND GETDATE() >= pd.SalesStartDate AND GETDATE() < pd.SalesEndDate, pd.Discount, 1) AS SalesPrice,
                                p.Description,
                                c.CategoryName,
                                STRING_AGG(t.TagName, ', ') AS TagNames,
                            	CONVERT(NVARCHAR, p.ProductShelfTime, 23) AS 'ProductShelfTime',
                            	p.ProductShelfTime,
                            	p.ProductStatus
                            FROM Product AS p
                            INNER JOIN TagConnect AS tc
                                ON p.ProductId = tc.ProductId
                            INNER JOIN Tags AS t
                                ON tc.TagId = t.TagId
                            INNER JOIN Categories AS c
                                ON p.CategoryId = c.CategoryId
                            LEFT JOIN ProductsDiscount AS pd
                                ON p.ProductId = pd.ProductId
                            GROUP BY 
                                p.ProductId, 
                                p.ProductName, 
                                p.ProductPrice, 
                                p.Description, 
                                c.CategoryName, 
                                pd.Discount, 
                                pd.SalesStartDate, 
                                pd.SalesEndDate,
                            	p.ProductStatus,
                            	p.ProductShelfTime

                        ";
            var result = await _dbConnection.QueryAsync<Products>(sql);
            return result;
        }
        public async Task<string> GetOrderResult(int userId)
        {
            var sql = @"
                        SELECT
                            o.UserId,
                            o.OrderId,
                            o.Orderdate,
                            ph.ProductName,
                            ph.SalesPrice,
                            IIF(o.PaymentTypeID=1,'綠界','LinePay') AS 'TransactionType'
                        FROM
                            PurchaseHistoryDetail AS ph
                        INNER JOIN [Order] AS o
                        ON ph.OrderId=o.OrderId
                        WHERE o.UserId=@UserId AND o.State=1
                       ";
            var result = await _dbConnection.QueryAsync<OrderResult>(sql, new { UserId = userId });
            var resultJson = JsonSerializer.Serialize(result);
            return resultJson;
        }

    }

}

