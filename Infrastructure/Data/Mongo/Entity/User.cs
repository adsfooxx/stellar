using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Mongo.Entity
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UseObjectrId { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("UserId")]
        public string UserId { get; set; }

        [BsonElement("Email")]
        public string Email { get; set; }

        [BsonElement("CreateAt")]
        public DateTime CreateAt { get; set; }

        [BsonElement("UpdateAt")]
        public DateTime UpdateAt { get; set; }
    }
}
