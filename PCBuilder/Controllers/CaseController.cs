using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCBuilder.Data;


namespace PCBuilder.Controllers
{
    public class CaseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CaseController(ApplicationDbContext context)
        {
            _context = context;
        }

    // GET: Case
    public async Task<IActionResult> Index(string searchString)
    {
        ViewData["CurrentFilter"] = searchString;
        var casesQuery = _context.Cases.AsQueryable();
        // --- KOMPATYBILNOŚĆ: OBUDOWA vs CHŁODZENIE CPU ---
        int? selectedCpuCoolerId = HttpContext.Session.GetInt32("SelectedCpuCoolerId");
        if (selectedCpuCoolerId != null)
        {
            var selectedCooler = await _context.CpuCoolers.FindAsync(selectedCpuCoolerId);
            if (selectedCooler != null)
            {
                // Pokazujemy tylko obudowy, które pomieszczą wysokość tego chłodzenia
                casesQuery = casesQuery.Where(c => c.MaxCpuCoolerHeightMm >= selectedCooler.HeightMm);
                ViewData["CompatibilityMessage"] = (ViewData["CompatibilityMessage"] != null ? ViewData["CompatibilityMessage"] + " oraz " : "") + 
                    $"mieszczące chłodzenie CPU o wysokości {selectedCooler.HeightMm}mm ({selectedCooler.Name}).";
            }
        }
        // --- NOWA KOMPATYBILNOŚĆ: OBUDOWA vs PŁYTA GŁÓWNA ---
        int? selectedMotherboardId = HttpContext.Session.GetInt32("SelectedMotherboardId");
        if (selectedMotherboardId != null)
        {
            var selectedMotherboard = await _context.Motherboards.FindAsync(selectedMotherboardId);
            if (selectedMotherboard != null && !string.IsNullOrEmpty(selectedMotherboard.FormFactor))
            {
                // 1. Zabezpieczamy to co wpisał użytkownik:
                // Usuwamy spacje, dodajemy przecinki i zamieniamy wszystko na MAŁE litery (ToLower)
                // Np. "Micro ATX" -> ",microatx,"
                string searchFactor = "," + selectedMotherboard.FormFactor.Replace(" ", "").ToLower() + ",";

                // 2. Szukamy w bazie stosując ten sam trik z dodaniem ToLower() dla kolumny bazy danych:
                casesQuery = casesQuery.Where(c => 
                    ("," + c.SupportedMoboFormFactors.Replace(" ", "").ToLower() + ",").Contains(searchFactor)
                );
                
                ViewData["CompatibilityMessage"] = $"Pokazuję obudowy kompatybilne z formatem płyty głównej {selectedMotherboard.FormFactor} ({selectedMotherboard.Name}).";
            }
        }

        // Istniejąca już wcześniej logika filtrowania dla długości GPU:
        int? selectedGpuId = HttpContext.Session.GetInt32("SelectedGpuId");
        if (selectedGpuId != null)
        {
            var selectedGpu = await _context.Gpus.FindAsync(selectedGpuId);
            if (selectedGpu != null && selectedGpu.Length.HasValue)
            {
                casesQuery = casesQuery.Where(c => c.MaxGpuLengthMm >= selectedGpu.Length.Value);
                ViewData["CompatibilityMessage"] = (ViewData["CompatibilityMessage"] != null ? ViewData["CompatibilityMessage"] + " oraz " : "") + $"mieszczące kartę o długości {selectedGpu.Length}mm.";
            }
        }

        if (!string.IsNullOrEmpty(searchString))
        {
            bool isNumber = double.TryParse(searchString, out double searchNumeric);
            casesQuery = casesQuery.Where(c => (c.Name != null && EF.Functions.Like(c.Name!, $"%{searchString}%")) || (c.Type != null && c.Type.Contains(searchString)) || (isNumber && c.MaxGpuLengthMm >= searchNumeric));
        }

        return View(await casesQuery.OrderBy(c => c.Name).ToListAsync());
    }

        // Opcjonalnie: Szczegóły obudowy
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var pcCase = await _context.Cases
                .FirstOrDefaultAsync(m => m.Id == id);

            if (pcCase == null) return NotFound();

            return View(pcCase);
        }
    }
}
