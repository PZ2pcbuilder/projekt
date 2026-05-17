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
            var query = _context.Gpus.AsQueryable();

            // --- KOMPATYBILNOŚĆ: GPU vs OBUDOWA ---
            int? selectedCaseId = HttpContext.Session.GetInt32("SelectedCaseId");
            if (selectedCaseId != null)
            {
                var selectedCase = await _context.Cases.FindAsync(selectedCaseId);
                if (selectedCase != null)
                {
                    // Pokazujemy tylko te karty, których długość jest mniejsza lub równa wolnej przestrzeni w obudowie
                    query = query.Where(g => g.Length <= selectedCase.MaxGpuLengthMm);
                    ViewData["CompatibilityMessage"] = $"Filtrowanie aktywne: Pokazuję karty o długości do {selectedCase.MaxGpuLengthMm}mm pasujące do obudowy {selectedCase.Name}.";
                }
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                bool isNumber = double.TryParse(searchString, out double searchNumeric);
                query = query.Where(s => (s.Name != null && s.Name.ToLower().Contains(searchString.ToLower())) || (s.Chipset != null && s.Chipset.ToLower().Contains(searchString.ToLower())) || (isNumber && s.Memory == searchNumeric));
            }

            return View(await query.ToListAsync());
        }
    }
}
