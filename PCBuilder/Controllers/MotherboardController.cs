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

            // --- NOWA KOMPATYBILNOŚĆ: PŁYTA GŁÓWNA vs OBUDOWA ---
            int? selectedCaseId = HttpContext.Session.GetInt32("SelectedCaseId");
            if (selectedCaseId != null)
            {
                var selectedCase = await _context.Cases.FindAsync(selectedCaseId);
                if (selectedCase != null && !string.IsNullOrEmpty(selectedCase.SupportedMoboFormFactors))
                {

                    query = query.Where(m => selectedCase.SupportedMoboFormFactors.Contains(m.FormFactor));
                    
                    ViewData["CompatibilityMessage"] = $"Pokazuję płyty główne pasujące do obudowy {selectedCase.Name} (Obsługiwane formaty: {selectedCase.SupportedMoboFormFactors.Replace("[","").Replace("]","").Replace("'","")}).";
                }
            }

            // Istniejąca już wcześniej logika filtrowania dla CPU:
            int? selectedCpuId = HttpContext.Session.GetInt32("SelectedCpuId");
            if (selectedCpuId != null)
            {
                var selectedCpu = await _context.Cpus.FindAsync(selectedCpuId);
                if (selectedCpu != null)
                {
                    query = query.Where(m => m.Socket == selectedCpu.Socket);
                    // Łączymy komunikaty, jeśli oba filtry są aktywne
                    ViewData["CompatibilityMessage"] = (ViewData["CompatibilityMessage"] != null ? ViewData["CompatibilityMessage"] + " oraz " : "") + $"pasujące do gniazda CPU: {selectedCpu.Socket}";
                }
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(m => m.Name.Contains(searchString) || m.FormFactor.Contains(searchString));
            }

            return View(await query.OrderBy(m => m.Name).ToListAsync());
        }
    }
}