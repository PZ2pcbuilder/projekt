using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCBuilder.Data;
using PCBuilder.Models.Filters;

namespace PCBuilder.Controllers
{
    public class CpuCoolerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CpuCoolerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(CpuCoolerFilter filter)
        {
            filter ??= new CpuCoolerFilter();
            var query = _context.CpuCoolers.AsQueryable();

            int? selectedCpuId = HttpContext.Session.GetInt32("SelectedCpuId");
            if (selectedCpuId != null)
            {
                var selectedCpu = await _context.Cpus.FindAsync(selectedCpuId);
                if (selectedCpu != null && !string.IsNullOrEmpty(selectedCpu.Socket))
                {
                    query = query.Where(cc => cc.SupportedSockets.Contains(selectedCpu.Socket));
                    ViewData["CompatibilityMessage"] = $"Kompatybilne z socketem {selectedCpu.Socket} ({selectedCpu.Name}).";
                }
            }

            int? selectedCaseId = HttpContext.Session.GetInt32("SelectedCaseId");
            if (selectedCaseId != null)
            {
                var selectedCase = await _context.Cases.FindAsync(selectedCaseId);
                if (selectedCase != null && selectedCase.MaxCpuCoolerHeightMm.HasValue)
                {
                    query = query.Where(cc => cc.HeightMm <= selectedCase.MaxCpuCoolerHeightMm.Value);
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var s = filter.Search.ToLower();
                query = query.Where(cc =>
                    cc.Name.ToLower().Contains(s) ||
                    cc.SupportedSockets.ToLower().Contains(s));
            }

            if (filter.MinPrice.HasValue) query = query.Where(cc => cc.Price >= filter.MinPrice);
            if (filter.MaxPrice.HasValue) query = query.Where(cc => cc.Price <= filter.MaxPrice);
            if (filter.MinRpm.HasValue) query = query.Where(cc => cc.Rpm >= filter.MinRpm);
            if (filter.MaxRpm.HasValue) query = query.Where(cc => cc.Rpm <= filter.MaxRpm);
            if (filter.MinNoise.HasValue) query = query.Where(cc => cc.NoiseLevel >= filter.MinNoise);
            if (filter.MaxNoise.HasValue) query = query.Where(cc => cc.NoiseLevel <= filter.MaxNoise);
            if (filter.MinHeight.HasValue) query = query.Where(cc => cc.HeightMm >= filter.MinHeight);
            if (filter.MaxHeight.HasValue) query = query.Where(cc => cc.HeightMm <= filter.MaxHeight);
            if (filter.MinSize.HasValue) query = query.Where(cc => cc.Size >= filter.MinSize);
            if (filter.MaxSize.HasValue) query = query.Where(cc => cc.Size <= filter.MaxSize);
            if (!string.IsNullOrWhiteSpace(filter.Color)) query = query.Where(cc => cc.Color == filter.Color);
            if (!string.IsNullOrWhiteSpace(filter.Socket)) query = query.Where(cc => cc.SupportedSockets.Contains(filter.Socket));

            var colors = await _context.CpuCoolers.Select(cc => cc.Color).Where(c => c != null).Distinct().OrderBy(c => c).ToListAsync();
            // Lista wszystkich gniazd – wyciągamy z procesorów (główne źródło prawdy)
            var sockets = await _context.Cpus.Select(c => c.Socket).Where(s => s != null).Distinct().OrderBy(s => s).ToListAsync();

            var vm = new CpuCoolerIndexViewModel
            {
                Items = await query.OrderBy(cc => cc.Name).ToListAsync(),
                Filter = filter,
                Colors = colors!,
                Sockets = sockets!
            };
            return View(vm);
        }
    }
}
