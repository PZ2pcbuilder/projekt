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
                    // 1. Zabezpieczamy listę z obudowy:
                    // Usuwamy nawiasy, apostrofy, spacje, zamieniamy na małe litery i otaczamy przecinkami
                    // Np. "['ATX', 'Micro ATX']" -> ",atx,microatx,"
                    string supportedFactorsCleaned = "," + selectedCase.SupportedMoboFormFactors
                        .Replace(" ", "")
                        .ToLower() + ",";

                    // 2. Filtrujemy płyty główne:
                    // Sprawdzamy czy nasza oczyszczona lista z obudowy (zmienna lokalna)
                    // ZAWIERA format płyty głównej (odpowiednio oczyszczony i otoczony przecinkami)
                    query = query.Where(m => 
                        supportedFactorsCleaned.Contains("," + m.FormFactor.Replace(" ", "").ToLower() + ",")
                    );
                    
                    // Zostawiłem Twoje wyświetlanie ładnego komunikatu bez zmian
                    ViewData["CompatibilityMessage"] = $"Pokazuję płyty główne pasujące do obudowy {selectedCase.Name} (Obsługiwane formaty: {selectedCase.SupportedMoboFormFactors}).";
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
                query = query.Where(m => (m.Name != null && m.Name.ToLower().Contains(searchString.ToLower())) || (m.FormFactor != null && m.FormFactor.ToLower().Contains(searchString.ToLower())));
            }

            return View(await query.OrderBy(m => m.Name).ToListAsync());
        }
    }
}
