using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCBuilder.Data;
using PCBuilder.Models;
using PCBuilder.Filters;

namespace PCBuilder.Controllers.Api
{
    [ApiController]
    [Route("api/data/gpus")]
    [ServiceFilter(typeof(ApiTokenFilter))]
    public class GpuApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GpuApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var gpus = await _context.Gpus.ToListAsync();
            return Ok(gpus);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var gpu = await _context.Gpus.FindAsync(id);
            if (gpu == null) return NotFound(new { error = "Nie znaleziono karty graficznej." });
            return Ok(gpu);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Gpu newGpu)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.Gpus.Add(newGpu);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = newGpu.Id }, newGpu);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Gpu updatedGpu)
        {
            if (id != updatedGpu.Id) return BadRequest(new { error = "Niezgodność ID w adresie i ciele żądania." });

            var dbGpu = await _context.Gpus.FindAsync(id);
            if (dbGpu == null) return NotFound();

            // Przepisanie wszystkich właściwości zgodnie z modelem Gpu.cs
            dbGpu.Name = updatedGpu.Name;
            dbGpu.Price = updatedGpu.Price;
            dbGpu.Chipset = updatedGpu.Chipset;
            dbGpu.Memory = updatedGpu.Memory;
            dbGpu.CoreClock = updatedGpu.CoreClock;
            dbGpu.BoostClock = updatedGpu.BoostClock;
            dbGpu.Color = updatedGpu.Color;
            dbGpu.Length = updatedGpu.Length;
            dbGpu.RecommendedPsuW = updatedGpu.RecommendedPsuW;
            dbGpu.PowerConnectors = updatedGpu.PowerConnectors;

            await _context.SaveChangesAsync();
            return Ok(dbGpu);
        }

        // 5. DELETE (DELETE /api/data/gpus/{id})
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var gpu = await _context.Gpus.FindAsync(id);
            if (gpu == null) return NotFound();

            _context.Gpus.Remove(gpu);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = $"Usunięto kartę graficzną o ID {id}" });
        }
    }
}