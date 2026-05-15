using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCBuilder.Data;

namespace PCBuilder.Controllers
{
    public class CpuCoolerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CpuCoolerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            var query = _context.CpuCoolers.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                bool isNumber = double.TryParse(searchString, out double searchNumeric);

                query = query.Where(c => 
                    c.Name.Contains(searchString) || 
                    c.SupportedSockets.Contains(searchString) ||
                    c.Color.Contains(searchString) ||
                    // Jeśli wpisano liczbę, szukaj chłodzeń o wysokości mniejszej lub równej (szukanie pod obudowę)
                    (isNumber && c.HeightMm <= searchNumeric)
                );
            }

            var result = await query.OrderBy(c => c.Name).ToListAsync();
            return View(result);
        }
    }
}