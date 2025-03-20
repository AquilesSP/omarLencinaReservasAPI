using Microsoft.AspNetCore.Mvc;
using OmarLencinaReservasAPI.Models;
using OmarLencinaReservasAPI.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OmarLencinaReservasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservaController : ControllerBase
    {
        private readonly ReservasContext _context;

        public ReservaController(ReservasContext context)
        {
            _context = context;
        }

        // GET: api/reserva
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reserva>>> GetReservas()
        {
            return await _context.Reservas.ToListAsync();
        }

        // GET: api/reserva/{numberId}
        [HttpGet("{numberId}")]
        public async Task<ActionResult<object>> GetReservasByUser(int numberId)
        {
            var today = DateTime.UtcNow.Date;

            var reservasUsuario = await _context.Reservas
                .Where(r => r.Cliente.Contains(numberId.ToString()) && r.FechaDesde.Date == today)
                .Select(r => new
                {
                    r.Id,
                    r.Cliente,
                    r.ServicioId,
                    r.FechaDesde,
                    r.FechaHasta,
                    ServicioNombre = _context.Servicios
                        .Where(s => s.Id == r.ServicioId)
                        .Select(s => s.Name)
                        .FirstOrDefault(),
                    ServicioColor = _context.Servicios
                        .Where(s => s.Id == r.ServicioId)
                        .Select(s => s.Color)
                        .FirstOrDefault()
                })
                .ToListAsync();

            if (!reservasUsuario.Any())
            {
                return NotFound(new { message = "No se encontraron reservas." });
            }

            var userData = new
            {
                Name = reservasUsuario.FirstOrDefault()?.Cliente.Split('-')[0].Trim(),
                NumberId = numberId,
                FullName = reservasUsuario.FirstOrDefault()?.Cliente
            };

            return Ok(new { userData, userReservas = reservasUsuario });
        }

        // POST: api/reserva
        [HttpPost]
        public async Task<ActionResult<Reserva>> PostReserva([FromBody] Reserva reserva)
        {
            if (reserva == null || string.IsNullOrEmpty(reserva.Cliente) || reserva.FechaDesde == default || reserva.FechaHasta == default)
            {
                return BadRequest("Los datos de la reserva no son válidos.");
            }

            // Extraer el código del cliente desde el campo 'cliente'
            var clienteCodigo = reserva.Cliente.Split('-').Last().Trim();

            // Validación: El cliente no puede tener dos reservas el mismo día para el mismo servicio
            var reservasClienteServicio = await _context.Reservas
                .Where(r => r.Cliente.Contains(clienteCodigo)  // Buscar por código de cliente
                            && r.ServicioId == reserva.ServicioId
                            && r.FechaDesde.Date == reserva.FechaDesde.Date)
                .ToListAsync();

            if (reservasClienteServicio.Any())
            {
                return BadRequest("Ya tenes una reserva para este servicio en este día.");
            }

            // Validación: No puede haber reservas solapadas para el mismo cliente y servicio
            var reservasSolapadas = await _context.Reservas
                .Where(r => r.ServicioId == reserva.ServicioId
                            && r.Cliente.Contains(clienteCodigo)  // Validar por código de cliente
                            && r.FechaDesde.Date == reserva.FechaDesde.Date
                            && ((r.FechaDesde < reserva.FechaHasta && r.FechaHasta > reserva.FechaDesde)))
                .ToListAsync();

            if (reservasSolapadas.Any())
            {
                return BadRequest("Ya existe una reserva para este cliente en el horario seleccionado.");
            }

            // Guardamos la reserva
            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            // Obtenemos el nombre del servicio
            var servicio = await _context.Servicios
                .Where(s => s.Id == reserva.ServicioId)
                .FirstOrDefaultAsync();

            if (servicio == null)
            {
                return BadRequest("El servicio seleccionado no existe.");
            }

            // Formatea el mensaje de éxito
            string message = $"Reserva exitosa: {reserva.Cliente} ha reservado el servicio de {servicio.Name}, el {reserva.FechaDesde:dddd} de {reserva.FechaDesde:HH:mm} a {reserva.FechaHasta:HH:mm} horas.";

            return Ok(new { reserva, message });
        }

        // DELETE: api/reserva/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReserva(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null)
            {
                return NotFound(new { message = "Reserva no encontrada." });
            }

            _context.Reservas.Remove(reserva);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Reserva eliminada correctamente." });
        }

    }
}
