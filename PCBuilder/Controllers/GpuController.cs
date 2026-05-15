using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCBuilder.Data;

namespace PCBuilder.Controllers
{
    public class GpuController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GpuController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Akcja wyświetlająca listę
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            var gpus = from g in _context.Gpus select g;

        if (!string.IsNullOrEmpty(searchString))
            {
                bool isNumber = double.TryParse(searchString, out double searchNumeric);

                gpus = gpus.Where(s => s.Name.Contains(searchString) 
                                    || s.Chipset.Contains(searchString)
                                    || (isNumber && s.Memory == searchNumeric));
            }
            return View(await gpus.ToListAsync());
        }
    }
}