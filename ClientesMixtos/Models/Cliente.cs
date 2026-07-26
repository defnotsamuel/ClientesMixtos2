using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace ClientesMixtos.Models
{

    public class Cliente
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ClienteId { get; set; } = string.Empty;

        [BsonElement("LIBRO 1")]
        public string Libro1 { get; set; } = string.Empty;

        [BsonElement("LIBRO 2")]
        public string Libro2 { get; set; } = string.Empty;

        [BsonElement("NOMBRE")]
        public string Nombre {  get; set; } = string.Empty;

        [BsonElement("VEHICULO")]
        public string Vehiculo { get; set; } = string.Empty;

        [BsonElement("PLACA")]
        public string Placa { get; set; } = string.Empty;

        [BsonElement("LOTE")]
        public string Lote { get; set; } = string.Empty;

        [BsonElement("FECHA DE COMPRA")]
        public string FechaDeCompra { get; set; } = string.Empty;

        [BsonElement("FECHA VENCE")]
        public string FechaVence { get; set; } = string.Empty;

        [BsonElement("FECHA PAGO")]
        public string FechaDePago { get; set; } = string.Empty;

        [BsonElement("FECHA MARCADA")]
        public string FechaMarcada { get; set; } = string.Empty;

        [BsonElement("REFRENDA")]
        public string Refrenda { get; set; } = string.Empty;

        [BsonElement("CIUDAD")]
        public string Ciudad { get; set; } = string.Empty;

        [BsonElement("TELEFONO")]
        public string Telefono { get; set; } = string.Empty;

        [BsonElement("RECUPERADO")]
        public bool Recuperado { get; set; }

        [BsonElement("PERDIDO")]
        public bool Perdido { get; set; }

        [BsonElement("ULTIMA FECHA PAGADA")]
        public string UltimaFechaPagada { get; set; } = string.Empty;

        [BsonIgnore]
        public ClienteState State { get; } = new ClienteState();
    }
}
