using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace ClientesMixtos.Models
{
    public class Nota
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public ObjectId ClienteId { get; set; }

        public string Descripcion { get; set; } = string.Empty;
        public string FechaCreacion {  get; set; } = string.Empty;

        [BsonIgnore]
        public NotaState State { get; } = new();
    }
}
