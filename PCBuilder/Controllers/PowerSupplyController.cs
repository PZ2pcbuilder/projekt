using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCBuilder.Data;

namespace PCBuilder.Controllers
{
    public class PowerSupplyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PowerSupplyController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            var query = _context.PowerSupplies.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                bool isNumber = int.TryParse(searchString, out int searchNumeric);

                query = query.Where(p => 
                    (p.Name != null && p.Name.ToLower().Contains(searchString.ToLower())) || 
                    (p.Efficiency != null && p.Efficiency.ToLower().Contains(searchString.ToLower())) || 
                    (p.Type != null && p.Type.ToLower().Contains(searchString.ToLower())) ||
                    (isNumber && p.Wattage >= searchNumeric));
            }

            var result = await query.OrderByDescending(p => p.Wattage).ToListAsync();
            return View(result);
        }
    }
}
