using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace ClientesMixtos.Models
{
    public class Nota
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.ObjectId)]
        public string ClienteId { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;
        public string FechaCreacion {  get; set; } = string.Empty;

        [BsonIgnore]
        public NotaState State { get; } = new();
    }
}
