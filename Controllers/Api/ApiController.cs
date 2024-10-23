using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using USMPWEB.Data;
using USMPWEB.Models;
using System.Threading.Tasks;

namespace USMPWEB.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlumnoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AlumnoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/alumno
        [HttpGet]
        public async Task<IActionResult> GetAlumnos()
        {
            var alumnos = await _context.DataAlumnos.ToListAsync();
            return Ok(alumnos);
        }

        // GET: api/alumno/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAlumno(int id)
        {
            var alumno = await _context.DataAlumnos.FindAsync(id);
            if (alumno == null)
            {
                return NotFound();
            }
            return Ok(alumno);
        }

        // POST: api/alumno
        [HttpPost]
        public async Task<IActionResult> PostAlumno([FromBody] Alumno alumno)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _context.DataAlumnos.AddAsync(alumno);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAlumno), new { id = alumno.Id }, alumno);
        }

        // PUT: api/alumno/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAlumno(int id, [FromBody] Alumno alumno)
        {
            if (id != alumno.Id)
            {
                return BadRequest();
            }

            _context.Entry(alumno).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.DataAlumnos.AnyAsync(e => e.Id == id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/alumno/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAlumno(int id)
        {
            var alumno = await _context.DataAlumnos.FindAsync(id);
            if (alumno == null)
            {
                return NotFound();
            }

            _context.DataAlumnos.Remove(alumno);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
