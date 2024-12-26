using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Mongo.Entity
{
    public class ChatDto
    {
        [BsonId]  // 將此屬性標記為 MongoDB 的 _id 字段
        [BsonRepresentation(BsonType.ObjectId)]  // 將 ObjectId 轉換為 string
        public string _Id { get; set; }
        public string User_Id { get; set; }
        public string Chat { get; set; }
    }
}
