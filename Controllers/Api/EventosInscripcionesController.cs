using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using USMPWEB.Data;
using USMPWEB.Models;
using System.Threading.Tasks;

namespace USMPWEB.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventosInscripcionesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EventosInscripcionesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/eventosinscripciones
        [HttpGet]
        public async Task<IActionResult> GetEventosInscripciones()
        {
            var eventosInscripciones = await _context.Set<EventosInscripciones>().Include(c => c.Categoria).Include(c => c.SubCategorias).ToListAsync();
            return Ok(eventosInscripciones);
        }

        // GET: api/eventosinscripciones/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventosInscripcion(long id)
        {
            var eventoInscripcion = await _context.Set<EventosInscripciones>()
                .Include(e => e.Categoria)
                .Include(e => e.SubCategorias)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (eventoInscripcion == null)
            {
                return NotFound();
            }
            return Ok(eventoInscripcion);
        }

        // POST: api/eventosinscripciones
        [HttpPost]
        public async Task<IActionResult> PostEventosInscripcion([FromBody] EventosInscripciones eventoInscripcion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _context.Set<EventosInscripciones>().AddAsync(eventoInscripcion);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEventosInscripcion), new { id = eventoInscripcion.Id }, eventoInscripcion);
        }

        // PUT: api/eventosinscripciones/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEventosInscripcion(long id, [FromBody] EventosInscripciones eventoInscripcion)
        {
            if (id != eventoInscripcion.Id)
            {
                return BadRequest();
            }

            _context.Entry(eventoInscripcion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Set<EventosInscripciones>().AnyAsync(e => e.Id == id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/eventosinscripciones/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEventosInscripcion(long id)
        {
            var eventoInscripcion = await _context.Set<EventosInscripciones>().FindAsync(id);
            if (eventoInscripcion == null)
            {
                return NotFound();
            }

            _context.Set<EventosInscripciones>().Remove(eventoInscripcion);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
