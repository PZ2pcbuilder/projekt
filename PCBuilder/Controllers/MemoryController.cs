using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCBuilder.Data;

namespace PCBuilder.Controllers
{
    public class MemoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MemoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            var query = _context.Memories.AsQueryable();

            // --- KOMPATYBILNOŚĆ: RAM vs PŁYTA GŁÓWNA ---
            int? selectedMotherboardId = HttpContext.Session.GetInt32("SelectedMotherboardId");
            if (selectedMotherboardId != null)
            {
                var selectedMotherboard = await _context.Motherboards.FindAsync(selectedMotherboardId);
                if (selectedMotherboard != null)
                {
                    // Filtry w bazie danych: Typ pamięci oraz limit maksymalnej pojemności płyty
                    query = query.Where(m => m.MemoryType == selectedMotherboard.MemoryType && 
                                             m.Capacity <= selectedMotherboard.MaxMemory);

                    // Pobieramy dane do pamięci, aby sprawdzić liczbę modułów (pole string 'Modules')
                    var memoryList = await query.ToListAsync();

                    // Filtracja lokalna: Liczba fizycznych kości w zestawie vs wolne sloty na płycie
                    memoryList = memoryList.Where(m => int.TryParse(m.Modules, out int count) && 
                                                       count <= selectedMotherboard.MemorySlots).ToList();

                    ViewData["CompatibilityMessage"] = $"Pokazuję pamięci ram pasujace do płyty {selectedMotherboard.Name}: Pokazuję RAM {selectedMotherboard.MemoryType}, maks. {selectedMotherboard.MaxMemory}GB, do {selectedMotherboard.MemorySlots} modułów.";
                    
                    // Wyszukiwanie tekstowe na przefiltrowanej liście lokalnej
                    if (!string.IsNullOrEmpty(searchString))
                    {
                        bool isNumber = double.TryParse(searchString, out double searchNumeric);
                        memoryList = memoryList.Where(m => 
                            m.Name.Contains(searchString) || 
                            m.Speed.Contains(searchString) || 
                            (isNumber && m.Capacity == searchNumeric)).ToList();
                    }

                    return View(memoryList);
                }
            }

            // --- STANDARDOWE WYSZUKIWANIE (gdy płyta nie jest wybrana) ---
            if (!string.IsNullOrEmpty(searchString))
            {
                bool isNumber = double.TryParse(searchString, out double searchNumeric);

                query = query.Where(m => 
                    m.Name.Contains(searchString) || 
                    m.MemoryType.Contains(searchString) ||
                    m.Speed.Contains(searchString) ||
                    (isNumber && m.Capacity == searchNumeric));
            }

            var result = await query.OrderByDescending(m => m.MemoryType).ThenBy(m => m.Name).ToListAsync();
            return View(result);
        }
    }
}