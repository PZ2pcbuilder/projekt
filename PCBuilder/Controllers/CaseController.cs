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
            // Przechowujemy frazę wyszukiwania, aby wróciła do pola tekstowego w widoku
            ViewData["CurrentFilter"] = searchString;

            // Startujemy z bazowym zapytaniem
            var casesQuery = _context.Cases.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                // Próbujemy sparsować wpisaną frazę na liczbę (np. dla długości GPU lub objętości)
                bool isNumber = double.TryParse(searchString, out double searchNumeric);

                casesQuery = casesQuery.Where(c => 
                    c.Name.Contains(searchString) || 
                    c.Type.Contains(searchString) || 
                    c.Color.Contains(searchString) ||
                    c.SupportedMoboFormFactors.Contains(searchString) ||
                    // Jeśli użytkownik wpisał liczbę, przeszukaj też parametry techniczne
                    (isNumber && (c.MaxGpuLengthMm >= searchNumeric || c.ExternalVolume == searchNumeric))
                );
            }

            // Pobieramy dane asynchronicznie i sortujemy np. po nazwie
            var result = await casesQuery.OrderBy(c => c.Name).ToListAsync();

            return View(result);
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