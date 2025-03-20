using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmarLencinaReservasAPI.Context;
using OmarLencinaReservasAPI.Models;

namespace OmarLencinaReservasAPI.Controllers
{
    [Route("api/servicios")]
    [ApiController]
    public class ServiciosController : ControllerBase
    {
        private readonly ReservasContext _context;

        public ServiciosController(ReservasContext context)
        {
            _context = context;
        }

        // GET: api/Servicios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Servicio>>> GetServicios()
        {
            return await _context.Servicios.ToListAsync();
        }

    }
}