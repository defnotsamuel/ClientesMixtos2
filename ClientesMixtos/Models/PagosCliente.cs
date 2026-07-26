using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace ClientesMixtos.Models
{
    public class PagosCliente
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.ObjectId)]
        public string ClienteId { get; set; } = string.Empty;

        public List<Pago> Pagos { get; set; } = [];

    }
}
