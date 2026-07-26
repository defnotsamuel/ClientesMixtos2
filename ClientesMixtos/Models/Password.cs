using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ClientesMixtos.Models
{
    public class Password
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? MongoId { get; set; }

        public string Usuario { get; set; } = string.Empty;
        public string EncryptedPin { get; set; } = string.Empty;
    }
}
