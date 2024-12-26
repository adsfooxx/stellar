using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Mongo.Entity
{
    public class Messages
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string MessageId { get; set; }

        [BsonElement("ConversationId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ConversationId { get; set; }

        [BsonElement("MessageType")]
        public MessageType MessageType { get; set; }

        [BsonElement("Content")]
        public string Content { get; set; }

        [BsonElement("Timestamp")]
        public DateTime Timestamp { get; set; }
    }

    public enum MessageType
    {
        System,
        User
    }
}
