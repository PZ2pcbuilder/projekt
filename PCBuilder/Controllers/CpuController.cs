using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCBuilder.Data;

namespace PCBuilder.Controllers
{
    public class CpuController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CpuController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Akcja wyświetlająca listę
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            var cpus = from c in _context.Cpus select c;

            if (!string.IsNullOrEmpty(searchString))
            {
                cpus = cpus.Where(s => s.Name.Contains(searchString) 
                                    || s.Socket.Contains(searchString)
                                    || s.Microarchitecture.Contains(searchString));
            }

            return View(await cpus.ToListAsync());
        }
    }
}