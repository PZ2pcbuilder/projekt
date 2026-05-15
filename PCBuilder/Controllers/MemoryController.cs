using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCBuilder.Data;

namespace PCBuilder.Controllers
{
    public class MemoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MemoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            var query = _context.Memories.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                bool isNumber = double.TryParse(searchString, out double searchNumeric);

                query = query.Where(m => 
                    m.Name.Contains(searchString) || 
                    m.MemoryType.Contains(searchString) ||
                    (isNumber && m.TotalCapacity == searchNumeric) || 
                    (isNumber && m.Speed == searchNumeric)
                );
            }

            var result = await query.OrderByDescending(m => m.MemoryType).ThenBy(m => m.Name).ToListAsync();
            return View(result);
        }
    }
}