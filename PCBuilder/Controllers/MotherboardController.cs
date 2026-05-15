using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCBuilder.Data;

namespace PCBuilder.Controllers
{
    public class MotherboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MotherboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            var query = _context.Motherboards.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(m => 
                    m.Name.Contains(searchString) || 
                    m.Socket.Contains(searchString) || 
                    m.MemoryType.Contains(searchString) ||
                    m.FormFactor.Contains(searchString));
            }

            // Sortujemy domyślnie po nazwie
            var result = await query.OrderBy(m => m.Name).ToListAsync();
            return View(result);
        }
    }
}