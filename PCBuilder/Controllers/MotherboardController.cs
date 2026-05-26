using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCBuilder.Data;
using PCBuilder.Models.Filters;


namespace PCBuilder.Controllers
{
    public class MotherboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MotherboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(MotherboardFilter filter)
        {
            filter ??= new MotherboardFilter();
            var query = _context.Motherboards.AsQueryable();

            int? selectedCaseId = HttpContext.Session.GetInt32("SelectedCaseId");
            if (selectedCaseId != null)
            {
                var selectedCase = await _context.Cases.FindAsync(selectedCaseId);
                if (selectedCase != null && !string.IsNullOrEmpty(selectedCase.SupportedMoboFormFactors))
                {
                    query = query.Where(m => selectedCase.SupportedMoboFormFactors.Contains(m.FormFactor));
                    ViewData["CompatibilityMessage"] = $"Pokazuję płyty pasujące do obudowy {selectedCase.Name}";
                }
            }

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
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var s = filter.Search.ToLower();
                query = query.Where(m =>
                    m.Name.ToLower().Contains(s) ||
                    m.Socket.ToLower().Contains(s) ||
                    m.MemoryType.ToLower().Contains(s) ||
                    m.FormFactor.ToLower().Contains(s));
            }

            if (filter.MinPrice.HasValue) query = query.Where(m => m.Price >= filter.MinPrice);
            if (filter.MaxPrice.HasValue) query = query.Where(m => m.Price <= filter.MaxPrice);
            if (filter.MinMemorySlots.HasValue) query = query.Where(m => m.MemorySlots >= filter.MinMemorySlots);
            if (filter.MaxMemorySlots.HasValue) query = query.Where(m => m.MemorySlots <= filter.MaxMemorySlots);
            if (filter.MinMaxMemory.HasValue) query = query.Where(m => m.MaxMemory >= filter.MinMaxMemory);
            if (filter.MaxMaxMemory.HasValue) query = query.Where(m => m.MaxMemory <= filter.MaxMaxMemory);
            if (filter.MinM2Slots.HasValue) query = query.Where(m => m.M2Slots >= filter.MinM2Slots);
            if (filter.MaxM2Slots.HasValue) query = query.Where(m => m.M2Slots <= filter.MaxM2Slots);
            if (filter.MinSataPorts.HasValue) query = query.Where(m => m.SataPorts >= filter.MinSataPorts);
            if (filter.MaxSataPorts.HasValue) query = query.Where(m => m.SataPorts <= filter.MaxSataPorts);
            if (!string.IsNullOrWhiteSpace(filter.Socket)) query = query.Where(m => m.Socket == filter.Socket);
            if (!string.IsNullOrWhiteSpace(filter.FormFactor)) query = query.Where(m => m.FormFactor == filter.FormFactor);
            if (!string.IsNullOrWhiteSpace(filter.MemoryType)) query = query.Where(m => m.MemoryType == filter.MemoryType);
            if (!string.IsNullOrWhiteSpace(filter.Color)) query = query.Where(m => m.Color == filter.Color);

            var sockets = await _context.Motherboards.Select(m => m.Socket).Distinct().OrderBy(s => s).ToListAsync();
            var formFactors = await _context.Motherboards.Select(m => m.FormFactor).Distinct().OrderBy(s => s).ToListAsync();
            var memTypes = await _context.Motherboards.Select(m => m.MemoryType).Distinct().OrderBy(s => s).ToListAsync();
            var colors = await _context.Motherboards.Select(m => m.Color).Distinct().OrderBy(s => s).ToListAsync();

            var vm = new MotherboardIndexViewModel
            {
                Items = await query.OrderBy(m => m.Name).ToListAsync(),
                Filter = filter,
                Sockets = sockets,
                FormFactors = formFactors,
                MemoryTypes = memTypes,
                Colors = colors
            };
            return View(vm);
        }
    }
}
