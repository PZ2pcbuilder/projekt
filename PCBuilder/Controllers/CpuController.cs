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
            var query = _context.Cpus.AsQueryable();

            // --- KOMPATYBILNOŚĆ W DRUGĄ STRONĘ ---
            int? selectedMotherboardId = HttpContext.Session.GetInt32("SelectedMotherboardId");
            if (selectedMotherboardId != null)
            {
                var selectedMotherboard = await _context.Motherboards.FindAsync(selectedMotherboardId);
                if (selectedMotherboard != null)
                {
                    // Pokazuj tylko procesory pasujące do wybranej płyty
                    query = query.Where(c => c.Socket == selectedMotherboard.Socket);
                    ViewData["CompatibilityMessage"] = $"Pokazuję procesory pasujące do płyty z gniazdem {selectedMotherboard.Socket}";
                }
            }
            // -------------------------------------

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(s => (s.Name != null && s.Name.ToLower().Contains(searchString.ToLower())) || (s.Socket != null && s.Socket.ToLower().Contains(searchString.ToLower())));
            }

            return View(await query.ToListAsync());
        }
    }
}
