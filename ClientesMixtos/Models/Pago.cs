using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientesMixtos.Models
{
    public class Pago
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string ClienteId { get; set; } = null!;

        public DateTime FechaPagada { get; set; }

        public DateTime? FechaMarcado { get; set; }
    }
}
