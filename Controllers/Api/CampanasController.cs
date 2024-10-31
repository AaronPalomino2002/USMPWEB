using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using USMPWEB.Data;
using USMPWEB.Models;
using System.Threading.Tasks;

namespace USMPWEB.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampanasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CampanasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // METODO GET: api/campanas
        [HttpGet]
        public async Task<IActionResult> GetCampanas()
        {
            var campanas = await _context.Set<Campanas>().Include(c => c.Categoria).Include(c => c.SubCategorias).ToListAsync();
            return Ok(campanas);
        }

        // GET: api/campanas/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCampana(int id)
        {
            var campana = await _context.Set<Campanas>()
                .Include(c => c.Categoria)
                .Include(c => c.SubCategorias)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (campana == null)
            {
                return NotFound();
            }
            return Ok(campana);
        }

        // POST: api/campanas
        [HttpPost]
        public async Task<IActionResult> PostCampana([FromBody] Campanas campana)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _context.Set<Campanas>().AddAsync(campana);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCampana), new { id = campana.Id }, campana);
        }

        // PUT: api/campanas/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCampana(int id, [FromBody] Campanas campana)
        {
            if (id != campana.Id)
            {
                return BadRequest();
            }

            _context.Entry(campana).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Set<Campanas>().AnyAsync(e => e.Id == id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/campanas/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCampana(int id)
        {
            var campana = await _context.Set<Campanas>().FindAsync(id);
            if (campana == null)
            {
                return NotFound();
            }

            _context.Set<Campanas>().Remove(campana);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
