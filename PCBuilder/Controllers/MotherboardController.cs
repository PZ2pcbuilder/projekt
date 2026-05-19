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

            // --- KOMPATYBILNOŚĆ: PŁYTA GŁÓWNA vs OBUDOWA ---
            int? selectedCaseId = HttpContext.Session.GetInt32("SelectedCaseId");
            if (selectedCaseId != null)
            {
                var selectedCase = await _context.Cases.FindAsync(selectedCaseId);
                if (selectedCase != null && !string.IsNullOrEmpty(selectedCase.SupportedMoboFormFactors))
                {
                    query = query.Where(m => selectedCase.SupportedMoboFormFactors.Contains(m.FormFactor));
                    ViewData["CompatibilityMessage"] = $"Pokazuję płyty główne pasujące do obudowy {selectedCase.Name} (Formaty: {selectedCase.SupportedMoboFormFactors.Replace("[","").Replace("]","").Replace("'","")})";
                }
            }

            // --- KOMPATYBILNOŚĆ: PŁYTA GŁÓWNA vs PROCESOR (CPU) ---
            int? selectedCpuId = HttpContext.Session.GetInt32("SelectedCpuId");
            if (selectedCpuId != null)
            {
                var selectedCpu = await _context.Cpus.FindAsync(selectedCpuId);
                if (selectedCpu != null)
                {
                    query = query.Where(m => m.Socket == selectedCpu.Socket);
                    ViewData["CompatibilityMessage"] = (ViewData["CompatibilityMessage"] != null ? ViewData["CompatibilityMessage"] + " oraz " : "") + $"pasujące do gniazda CPU: {selectedCpu.Socket}";
                }
            }

            // --- KOMPATYBILNOŚĆ: PŁYTA GŁÓWNA vs PAMIĘĆ RAM ---
            int? selectedMemoryId = HttpContext.Session.GetInt32("SelectedMemoryId");
            if (selectedMemoryId != null)
            {
                var selectedMemory = await _context.Memories.FindAsync(selectedMemoryId);
                if (selectedMemory != null)
                {
                    int.TryParse(selectedMemory.Modules, out int requiredSlots);

                    query = query.Where(m => m.MemoryType == selectedMemory.MemoryType &&
                                             m.MaxMemory >= selectedMemory.Capacity &&
                                             m.MemorySlots >= requiredSlots);

                    ViewData["CompatibilityMessage"] = (ViewData["CompatibilityMessage"] != null ? ViewData["CompatibilityMessage"] + " oraz " : "") + 
                        $"obsługujące RAM ({selectedMemory.Name}: {selectedMemory.Capacity}GB, modułów: {requiredSlots})";
                }
            }

            // --- WYSZUKIWANIE TEKSTOWE ---
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(m => 
                    m.Name.Contains(searchString) || 
                    m.Socket.Contains(searchString) || 
                    m.MemoryType.Contains(searchString) ||
                    m.FormFactor.Contains(searchString));
            }

            var result = await query.OrderBy(m => m.Name).ToListAsync();
            return View(result);
        }
    }
}