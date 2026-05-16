using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCBuilder.Data;
using System.Linq;
using System.Threading.Tasks;

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

        // --- NOWA KOMPATYBILNOŚĆ: OBUDOWA vs PŁYTA GŁÓWNA ---
        int? selectedMotherboardId = HttpContext.Session.GetInt32("SelectedMotherboardId");
        if (selectedMotherboardId != null)
        {
            var selectedMotherboard = await _context.Motherboards.FindAsync(selectedMotherboardId);
            if (selectedMotherboard != null && !string.IsNullOrEmpty(selectedMotherboard.FormFactor))
            {
                // Szukamy obudów, które w stringu z formatami mają zapisany format naszej płyty
                casesQuery = casesQuery.Where(c => c.SupportedMoboFormFactors.Contains(selectedMotherboard.FormFactor));
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
            casesQuery = casesQuery.Where(c => c.Name.Contains(searchString) || c.Type.Contains(searchString) || (isNumber && c.MaxGpuLengthMm >= searchNumeric));
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