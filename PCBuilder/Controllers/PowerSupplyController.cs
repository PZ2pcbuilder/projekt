using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCBuilder.Data;
using PCBuilder.Models.Filters;

namespace PCBuilder.Controllers
{
    public class PowerSupplyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PowerSupplyController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(PowerSupplyFilter filter)
        {
            filter ??= new PowerSupplyFilter();
            var query = _context.PowerSupplies.AsQueryable();

            int? cpuId = HttpContext.Session.GetInt32("SelectedCpuId");
            int? gpuId = HttpContext.Session.GetInt32("SelectedGpuId");
            int calculatedMinimum = 0;
            int gpuRecommendedPsu = 0;

            if (cpuId != null)
            {
                var cpu = await _context.Cpus.FindAsync(cpuId);
                if (cpu != null) calculatedMinimum += cpu.Tdp;
            }
            if (gpuId != null)
            {
                var gpu = await _context.Gpus.FindAsync(gpuId);
                if (gpu != null) gpuRecommendedPsu = gpu.RecommendedPsuW;
            }

            if (calculatedMinimum > 0 || gpuRecommendedPsu > 0)
            {
                calculatedMinimum += 70 + gpuRecommendedPsu;
                int calculatedRecommended = (int)Math.Ceiling(calculatedMinimum * 1.3 / 50.0) * 50;
                query = query.Where(p => p.Wattage >= calculatedRecommended);
                ViewData["CompatibilityMessage"] = $"Pokazuję zasilacze min. {calculatedRecommended}W (wyliczone na podstawie podzespołów)";
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var s = filter.Search.ToLower();
                query = query.Where(p =>
                    (p.Name != null && p.Name.ToLower().Contains(s)) ||
                    (p.Efficiency != null && p.Efficiency.ToLower().Contains(s)) ||
                    (p.Type != null && p.Type.ToLower().Contains(s)));
            }

            if (filter.MinPrice.HasValue) query = query.Where(p => p.Price >= filter.MinPrice);
            if (filter.MaxPrice.HasValue) query = query.Where(p => p.Price <= filter.MaxPrice);
            if (filter.MinWattage.HasValue) query = query.Where(p => p.Wattage >= filter.MinWattage);
            if (filter.MaxWattage.HasValue) query = query.Where(p => p.Wattage <= filter.MaxWattage);
            if (filter.MinPcie.HasValue) query = query.Where(p => p.Pcie6Plus2Connectors >= filter.MinPcie);
            if (filter.MaxPcie.HasValue) query = query.Where(p => p.Pcie6Plus2Connectors <= filter.MaxPcie);
            if (!string.IsNullOrWhiteSpace(filter.Type)) query = query.Where(p => p.Type == filter.Type);
            if (!string.IsNullOrWhiteSpace(filter.Efficiency)) query = query.Where(p => p.Efficiency == filter.Efficiency);
            if (!string.IsNullOrWhiteSpace(filter.Modular)) query = query.Where(p => p.Modular == filter.Modular);
            if (!string.IsNullOrWhiteSpace(filter.Color)) query = query.Where(p => p.Color == filter.Color);
            if (!string.IsNullOrWhiteSpace(filter.FormFactor)) query = query.Where(p => p.FormFactor == filter.FormFactor);

            var types = await _context.PowerSupplies.Select(p => p.Type).Distinct().OrderBy(s => s).ToListAsync();
            var effs = await _context.PowerSupplies.Select(p => p.Efficiency).Where(s => s != null).Distinct().OrderBy(s => s).ToListAsync();
            var modulars = await _context.PowerSupplies.Select(p => p.Modular).Distinct().OrderBy(s => s).ToListAsync();
            var colors = await _context.PowerSupplies.Select(p => p.Color).Where(s => s != null).Distinct().OrderBy(s => s).ToListAsync();
            var formFactors = await _context.PowerSupplies.Select(p => p.FormFactor).Distinct().OrderBy(s => s).ToListAsync();

            var vm = new PowerSupplyIndexViewModel
            {
                Items = await query.OrderBy(p => p.Wattage).ToListAsync(),
                Filter = filter,
                Types = types,
                Efficiencies = effs!,
                Modulars = modulars,
                Colors = colors!,
                FormFactors = formFactors
            };
            return View(vm);
        }
    }
}
