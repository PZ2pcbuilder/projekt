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

            // --- KOMPATYBILNOŚĆ: CHŁODZENIE vs PROCESOR ---
            int? selectedCpuId = HttpContext.Session.GetInt32("SelectedCpuId");
            if (selectedCpuId != null)
            {
                var selectedCpu = await _context.Cpus.FindAsync(selectedCpuId);
                if (selectedCpu != null && !string.IsNullOrEmpty(selectedCpu.Socket))
                {
                    // Sprawdzamy, czy string z obsługiwanymi socketami zawiera socket wybranego CPU
                    query = query.Where(cc => cc.SupportedSockets.Contains(selectedCpu.Socket));
                    ViewData["CompatibilityMessage"] = $"Pokazuję chłodzenia kompatybilne z socketem {selectedCpu.Socket} ({selectedCpu.Name}).";
                }
            }

            // --- KOMPATYBILNOŚĆ: CHŁODZENIE vs OBUDOWA ---
            int? selectedCaseId = HttpContext.Session.GetInt32("SelectedCaseId");
            if (selectedCaseId != null)
            {
                var selectedCase = await _context.Cases.FindAsync(selectedCaseId);
                if (selectedCase != null && selectedCase.MaxCpuCoolerHeightMm.HasValue)
                {
                    // Wysokość chłodzenia musi być mniejsza bądź równa maksymalnej dopuszczalnej w obudowie
                    query = query.Where(cc => cc.HeightMm <= selectedCase.MaxCpuCoolerHeightMm.Value);
                    
                    ViewData["CompatibilityMessage"] = (ViewData["CompatibilityMessage"] != null ? ViewData["CompatibilityMessage"] + " oraz " : "") + 
                        $"mieszczące się w obudowie {selectedCase.Name} (wysokość do {selectedCase.MaxCpuCoolerHeightMm}mm).";
                }
            }

            // --- WYSZUKIWANIE TEKSTOWE ---
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(cc => 
                    cc.Name.ToLower().Contains(searchString.ToLower()) || 
                    cc.SupportedSockets.ToLower().Contains(searchString.ToLower()));
            }

            var result = await query.OrderBy(cc => cc.Name).ToListAsync();
            return View(result);
        }
    }
}