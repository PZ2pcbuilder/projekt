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

            if (!string.IsNullOrEmpty(searchString))
            {
                bool isNumber = double.TryParse(searchString, out double searchNumeric);

                query = query.Where(s => 
                    (s.Name != null && s.Name.ToLower().Contains(searchString.ToLower())) || 
                    (s.Type != null && s.Type.ToLower().Contains(searchString.ToLower())) || 
                    (s.Interface != null && s.Interface.ToLower().Contains(searchString.ToLower())) ||
                    (isNumber && s.Capacity >= searchNumeric));
            }

            var result = await query.OrderByDescending(s => s.Type).ThenByDescending(s => s.Capacity).ToListAsync();
            return View(result);
        }
    }
}
