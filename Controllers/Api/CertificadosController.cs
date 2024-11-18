using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using USMPWEB.Data;
using USMPWEB.Models;
using System.Threading.Tasks;

namespace USMPWEB.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificadosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CertificadosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/certificados
        [HttpGet]
        public async Task<IActionResult> GetCertificados()
        {
            var certificados = await _context.Set<Certificados>().Include(c => c.Categoria).Include(c => c.SubCategorias).ToListAsync();
            return Ok(certificados);
        }

        // GET: api/certificados/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCertificado(long id)
        {
            var certificado = await _context.Set<Certificados>()
                .Include(c => c.Categoria)
                .Include(c => c.SubCategorias)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (certificado == null)
            {
                return NotFound();
            }
            return Ok(certificado);
        }

        // POST: api/certificados
        [HttpPost]
        public async Task<IActionResult> PostCertificado([FromBody] Certificados certificado)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _context.Set<Certificados>().AddAsync(certificado);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCertificado), new { id = certificado.Id }, certificado);
        }

        // PUT: api/certificados/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCertificado(long id, [FromBody] Certificados certificado)
        {
            if (id != certificado.Id)
            {
                return BadRequest();
            }

            _context.Entry(certificado).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Set<Certificados>().AnyAsync(e => e.Id == id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/certificados/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCertificado(long id)
        {
            var certificado = await _context.Set<Certificados>().FindAsync(id);
            if (certificado == null)
            {
                return NotFound();
            }

            _context.Set<Certificados>().Remove(certificado);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
