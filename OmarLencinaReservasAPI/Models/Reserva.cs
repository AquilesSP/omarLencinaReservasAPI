using System.ComponentModel.DataAnnotations;

namespace OmarLencinaReservasAPI.Models
{
    public class Reserva
    {
        public int Id { get; set; }

        public required string Cliente { get; set; }

        public required int ServicioId { get; set; }

        public required DateTime FechaDesde { get; set; }
        public required DateTime FechaHasta { get; set; }
    }
}
