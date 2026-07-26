using System;

namespace ClientesMixtos.Models
{
    public class ClienteState
    {
        public DateTime? FechaMarcada { get; set; }
        public DateTime? FechaDeCompra { get; set; }
        public DateTime? FechaDePago { get; set; }
        public DateTime? FechaVence { get; set; }
        public List<Pago> FechasPagadas { get; set; } = [];

        public bool MarcadoEsteMes {  get; set; }
        public bool PagoEsteMes { get; set; }
        public bool Pendiente { get; set; }
        public int MesesAtrasado { get; set; }
        public bool EstaAtrasado => MesesAtrasado > 0;
        public bool AtrasoGrave => MesesAtrasado >= 2;
        public string MesesAtrasadoText => MesesAtrasado > 0 ? $"{MesesAtrasado} m" : "";
    }
}
