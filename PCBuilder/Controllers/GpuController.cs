using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCBuilder.Data;
using PCBuilder.Models.Filters;


namespace PCBuilder.Controllers
{
    public class GpuController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GpuController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(GpuFilter filter)
        {
            filter ??= new GpuFilter();
            var query = _context.Gpus.AsQueryable();

            int? selectedCaseId = HttpContext.Session.GetInt32("SelectedCaseId");
            if (selectedCaseId != null)
            {
                var selectedCase = await _context.Cases.FindAsync(selectedCaseId);
                if (selectedCase != null && selectedCase.MaxGpuLengthMm.HasValue)
                {
                    query = query.Where(g => g.Length <= selectedCase.MaxGpuLengthMm);
                    ViewData["CompatibilityMessage"] = $"Pokazuję karty o długości do {selectedCase.MaxGpuLengthMm}mm pasujące do obudowy {selectedCase.Name}.";
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var s = filter.Search.ToLower();
                query = query.Where(g =>
                    (g.Name != null && g.Name.ToLower().Contains(s)) ||
                    (g.Chipset != null && g.Chipset.ToLower().Contains(s)));
            }

            if (filter.MinPrice.HasValue) query = query.Where(g => g.Price >= filter.MinPrice);
            if (filter.MaxPrice.HasValue) query = query.Where(g => g.Price <= filter.MaxPrice);
            if (filter.MinMemory.HasValue) query = query.Where(g => g.Memory >= filter.MinMemory);
            if (filter.MaxMemory.HasValue) query = query.Where(g => g.Memory <= filter.MaxMemory);
            if (filter.MinLength.HasValue) query = query.Where(g => g.Length >= filter.MinLength);
            if (filter.MaxLength.HasValue) query = query.Where(g => g.Length <= filter.MaxLength);
            if (filter.MinBoost.HasValue) query = query.Where(g => g.BoostClock >= filter.MinBoost);
            if (filter.MaxBoost.HasValue) query = query.Where(g => g.BoostClock <= filter.MaxBoost);
            if (filter.MinPsu.HasValue) query = query.Where(g => g.RecommendedPsuW >= filter.MinPsu);
            if (filter.MaxPsu.HasValue) query = query.Where(g => g.RecommendedPsuW <= filter.MaxPsu);
            if (!string.IsNullOrWhiteSpace(filter.Chipset)) query = query.Where(g => g.Chipset == filter.Chipset);
            if (!string.IsNullOrWhiteSpace(filter.Color)) query = query.Where(g => g.Color == filter.Color);

            var chipsets = await _context.Gpus.Select(g => g.Chipset).Where(c => c != null).Distinct().OrderBy(c => c).ToListAsync();
            var colors = await _context.Gpus.Select(g => g.Color).Where(c => c != null).Distinct().OrderBy(c => c).ToListAsync();

            var vm = new GpuIndexViewModel
            {
                Items = await query.OrderBy(g => g.Name).ToListAsync(),
                Filter = filter,
                Chipsets = chipsets!,
                Colors = colors!
            };
            return View(vm);
        }
    }
}
