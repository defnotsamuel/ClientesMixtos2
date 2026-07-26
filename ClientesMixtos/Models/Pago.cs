using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientesMixtos.Models
{
    public class Pago
    {
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime FechaPagada { get; set; }
        public DateTime FechaMarcado { get; set; }

    }
}
