using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Infrastructure.Data.Mongo.Entity
{
    public class Products
    {
        [BsonId]  // 將此屬性標記為 MongoDB 的 _id 字段
        [BsonRepresentation(BsonType.ObjectId)]  // 將 ObjectId 轉換為 string
        public string _Id { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal? ProductPrice { get; set; }
        public decimal? SalesPrice { get; set; }
        public string? Description { get; set; }
        public string? CategoryName {  get; set; }
        public string? TagNames { get; set; }
        public string? ProductShelfTime {  get; set; }
        public string? ProductStatus { get; set;}
    }
}
