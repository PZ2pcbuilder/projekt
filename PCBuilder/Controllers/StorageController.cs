using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCBuilder.Data;

namespace PCBuilder.Controllers
{
    public class StorageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StorageController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            var query = _context.Storages.AsQueryable();

            // --- WYSZUKIWANIE TEKSTOWE ---
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(s => 
                    s.Name.Contains(searchString) || 
                    s.Type.Contains(searchString) || 
                    s.Interface.Contains(searchString) || 
                    s.FormFactor.Contains(searchString));
            }

            var result = await query.OrderBy(s => s.Type).ThenBy(s => s.Name).ToListAsync();
            return View(result);
        }
    }
}