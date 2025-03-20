using Microsoft.EntityFrameworkCore;
using OmarLencinaReservasAPI.Models;

namespace OmarLencinaReservasAPI.Context
{
    public class ReservasContext : DbContext
    {
        public ReservasContext(DbContextOptions<ReservasContext> options) : base(options) { }

        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<Reserva> Reservas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Servicio>().HasData(
                new Servicio { Id = 1, Name = "Spa", Color = "#3CB371" },
                new Servicio { Id = 2, Name = "Gimnasio", Color = "#FFD700" },
                new Servicio { Id = 3, Name = "Piscina", Color = "#1E90FF" }
            );
        }
    }
}
