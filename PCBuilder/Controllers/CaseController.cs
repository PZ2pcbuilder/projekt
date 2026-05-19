using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCBuilder.Data;
using PCBuilder.Models.Filters;

namespace PCBuilder.Controllers
{
    public class CaseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CaseController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(CaseFilter filter)
        {
            filter ??= new CaseFilter();
            var query = _context.Cases.AsQueryable();

            int? selectedCpuCoolerId = HttpContext.Session.GetInt32("SelectedCpuCoolerId");
            if (selectedCpuCoolerId != null)
            {
                var selectedCooler = await _context.CpuCoolers.FindAsync(selectedCpuCoolerId);
                if (selectedCooler != null)
                {
                    query = query.Where(c => c.MaxCpuCoolerHeightMm >= selectedCooler.HeightMm);
                    ViewData["CompatibilityMessage"] = $"Mieszczące chłodzenie {selectedCooler.Name} ({selectedCooler.HeightMm}mm).";
                }
            }

            int? selectedMotherboardId = HttpContext.Session.GetInt32("SelectedMotherboardId");
            if (selectedMotherboardId != null)
            {
                var selectedMotherboard = await _context.Motherboards.FindAsync(selectedMotherboardId);
                if (selectedMotherboard != null && !string.IsNullOrEmpty(selectedMotherboard.FormFactor))
                {
                    string searchFactor = "," + selectedMotherboard.FormFactor.Replace(" ", "").ToLower() + ",";
                    query = query.Where(c =>
                        ("," + c.SupportedMoboFormFactors.Replace(" ", "").ToLower() + ",").Contains(searchFactor));
                    ViewData["CompatibilityMessage"] = $"Pasujące do płyty {selectedMotherboard.Name} ({selectedMotherboard.FormFactor}).";
                }
            }

            int? selectedGpuId = HttpContext.Session.GetInt32("SelectedGpuId");
            if (selectedGpuId != null)
            {
                var selectedGpu = await _context.Gpus.FindAsync(selectedGpuId);
                if (selectedGpu != null && selectedGpu.Length.HasValue)
                {
                    query = query.Where(c => c.MaxGpuLengthMm >= selectedGpu.Length.Value);
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var s = filter.Search.ToLower();
                query = query.Where(c =>
                    c.Name.ToLower().Contains(s) ||
                    c.Type.ToLower().Contains(s));
            }

            if (filter.MinPrice.HasValue) query = query.Where(c => c.Price >= filter.MinPrice);
            if (filter.MaxPrice.HasValue) query = query.Where(c => c.Price <= filter.MaxPrice);
            if (filter.MinVolume.HasValue) query = query.Where(c => c.ExternalVolume >= filter.MinVolume);
            if (filter.MaxVolume.HasValue) query = query.Where(c => c.ExternalVolume <= filter.MaxVolume);
            if (filter.MinMaxGpuLength.HasValue) query = query.Where(c => c.MaxGpuLengthMm >= filter.MinMaxGpuLength);
            if (filter.MaxMaxGpuLength.HasValue) query = query.Where(c => c.MaxGpuLengthMm <= filter.MaxMaxGpuLength);
            if (filter.MinMaxCoolerHeight.HasValue) query = query.Where(c => c.MaxCpuCoolerHeightMm >= filter.MinMaxCoolerHeight);
            if (filter.MaxMaxCoolerHeight.HasValue) query = query.Where(c => c.MaxCpuCoolerHeightMm <= filter.MaxMaxCoolerHeight);
            if (!string.IsNullOrWhiteSpace(filter.Type)) query = query.Where(c => c.Type == filter.Type);
            if (!string.IsNullOrWhiteSpace(filter.Color)) query = query.Where(c => c.Color == filter.Color);
            if (!string.IsNullOrWhiteSpace(filter.PsuFormFactor)) query = query.Where(c => c.PsuFormFactor == filter.PsuFormFactor);

            var types = await _context.Cases.Select(c => c.Type).Distinct().OrderBy(s => s).ToListAsync();
            var colors = await _context.Cases.Select(c => c.Color).Distinct().OrderBy(s => s).ToListAsync();
            var psuFFs = await _context.Cases.Select(c => c.PsuFormFactor).Distinct().OrderBy(s => s).ToListAsync();

            var vm = new CaseIndexViewModel
            {
                Items = await query.OrderBy(c => c.Name).ToListAsync(),
                Filter = filter,
                Types = types,
                Colors = colors,
                PsuFormFactors = psuFFs
            };
            return View(vm);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var pcCase = await _context.Cases.FirstOrDefaultAsync(m => m.Id == id);
            if (pcCase == null) return NotFound();
            return View(pcCase);
        }
    }
}
