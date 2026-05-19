using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCBuilder.Data;

namespace PCBuilder.Controllers
{
    public class PowerSupplyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PowerSupplyController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            var query = _context.PowerSupplies.AsQueryable();
            
            int? cpuId = HttpContext.Session.GetInt32("SelectedCpuId");
            int? gpuId = HttpContext.Session.GetInt32("SelectedGpuId");

            int calculatedMinimum = 0;
            int gpuRecommendedPsu = 0;

            // 1. Zliczamy pobór mocy z CPU
            if (cpuId != null)
            {
                var cpu = await _context.Cpus.FindAsync(cpuId);
                if (cpu != null) 
                {
                    // Upewnij się, czy Twoje pole Tdp to int, czy int? (jeśli int?, dodaj ?? 0)
                    calculatedMinimum += cpu.Tdp; 
                }
            }
            if (gpuId != null)
            {
                var gpu = await _context.Gpus.FindAsync(gpuId);
                if (gpu != null) 
                {
                    gpuRecommendedPsu = gpu.RecommendedPsuW;
                }
            }

            if (calculatedMinimum > 0 || gpuRecommendedPsu > 0)
            {
                calculatedMinimum += 70;
                calculatedMinimum += gpuRecommendedPsu; 
                
                int calculatedRecommended = (int)Math.Ceiling(calculatedMinimum * 1.3 / 50.0) * 50;

                query = query.Where(p => p.Wattage >= calculatedRecommended);

                ViewData["CompatibilityMessage"] = $"Pokazuję zasilacze, które mają minimum {calculatedRecommended}W (wyliczone na podstawie podzespołów)";
            }

            // --- WYSZUKIWANIE TEKSTOWE I RĘCZNE FILTROWANIE MOCY ---
            if (!string.IsNullOrEmpty(searchString))
            {
                bool isNumber = int.TryParse(searchString, out int searchNumeric);

                query = query.Where(p => 
                    (p.Name != null && p.Name.ToLower().Contains(searchString.ToLower())) || 
                    (p.Efficiency != null && p.Efficiency.ToLower().Contains(searchString.ToLower())) || 
                    (p.Type != null && p.Type.ToLower().Contains(searchString.ToLower())) ||
                    (isNumber && p.Wattage >= searchNumeric));
            }

            // Zmieniono na sortowanie rosnące (p.Wattage) - użytkownik zazwyczaj szuka najpierw tańszych/słabszych zasilaczy spełniających kryteria, a nie potworów 1600W
            var result = await query.OrderBy(p => p.Wattage).ToListAsync();
            return View(result);
        }
    }
}