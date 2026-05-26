using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCBuilder.Data;
using PCBuilder.Models;
using PCBuilder.Filters;


namespace PCBuilder.Controllers.Api
{
    // Oznacza, że to jest REST API (zwraca JSON, a nie widoki HTML)
    [ApiController]
    [Route("api/data/cpus")]
    // Narzuca nasz nowy filtr sprawdzający nagłówki z tokenem!
    [ServiceFilter(typeof(ApiTokenFilter))] 
    public class CpuApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CpuApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Wyświetlanie danych
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cpus = await _context.Cpus.ToListAsync();
            return Ok(cpus);
        }

        // POST: Dodawanie danych
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Cpu newCpu)
        {
            _context.Cpus.Add(newCpu);
            await _context.SaveChangesAsync();
            return Ok(newCpu);
        }

        // PUT: Modyfikacja
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Cpu updatedCpu)
        {
            if (id != updatedCpu.Id) return BadRequest();
            
            _context.Entry(updatedCpu).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(updatedCpu);
        }

        // DELETE: Usuwanie
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var cpu = await _context.Cpus.FindAsync(id);
            if (cpu == null) return NotFound();

            _context.Cpus.Remove(cpu);
            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }
    }
}
