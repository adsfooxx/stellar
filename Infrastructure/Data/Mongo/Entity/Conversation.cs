using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Mongo.Entity
{
    public class Conversation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ConversationId { get; set; }

        [BsonElement("UserId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

        [BsonElement("Summarization")]
        public string Summarization { get; set; }

        [BsonElement("CreateAt")]
        public DateTime CreateAt { get; set; }

        [BsonElement("UpdateAt")]
        public DateTime UpdateAt { get; set; }
    }
}
