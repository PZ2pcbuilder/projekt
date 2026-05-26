using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCBuilder.Data;
using PCBuilder.Models.Filters;


namespace PCBuilder.Controllers
{
    public class MemoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MemoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(MemoryFilter filter)
        {
            filter ??= new MemoryFilter();
            var query = _context.Memories.AsQueryable();

            int? selectedMotherboardId = HttpContext.Session.GetInt32("SelectedMotherboardId");
            if (selectedMotherboardId != null)
            {
                var mb = await _context.Motherboards.FindAsync(selectedMotherboardId);
                if (mb != null)
                {
                    query = query.Where(m => m.MemoryType == mb.MemoryType && m.Capacity <= mb.MaxMemory);
                    ViewData["CompatibilityMessage"] = $"Pokazuję RAM pasujące do płyty {mb.Name} ({mb.MemoryType}, max {mb.MaxMemory}GB).";
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var s = filter.Search.ToLower();
                query = query.Where(m =>
                    m.Name.ToLower().Contains(s) ||
                    m.MemoryType.ToLower().Contains(s));
            }

            if (filter.MinPrice.HasValue) query = query.Where(m => m.Price >= filter.MinPrice);
            if (filter.MaxPrice.HasValue) query = query.Where(m => m.Price <= filter.MaxPrice);
            if (filter.MinCapacity.HasValue) query = query.Where(m => m.Capacity >= filter.MinCapacity);
            if (filter.MaxCapacity.HasValue) query = query.Where(m => m.Capacity <= filter.MaxCapacity);
            if (filter.MinSpeed.HasValue) query = query.Where(m => m.Speed >= filter.MinSpeed);
            if (filter.MaxSpeed.HasValue) query = query.Where(m => m.Speed <= filter.MaxSpeed);
            if (!string.IsNullOrWhiteSpace(filter.MemoryType)) query = query.Where(m => m.MemoryType == filter.MemoryType);
            if (!string.IsNullOrWhiteSpace(filter.Color)) query = query.Where(m => m.Color == filter.Color);
            if (!string.IsNullOrWhiteSpace(filter.Modules)) query = query.Where(m => m.Modules == filter.Modules);

            var memTypes = await _context.Memories.Select(m => m.MemoryType).Distinct().OrderBy(s => s).ToListAsync();
            var colors = await _context.Memories.Select(m => m.Color).Distinct().OrderBy(s => s).ToListAsync();
            var modulesList = await _context.Memories.Select(m => m.Modules).Distinct().OrderBy(s => s).ToListAsync();

            var vm = new MemoryIndexViewModel
            {
                Items = await query.OrderByDescending(m => m.MemoryType).ThenBy(m => m.Name).ToListAsync(),
                Filter = filter,
                MemoryTypes = memTypes,
                Colors = colors,
                ModulesList = modulesList
            };
            return View(vm);
        }
    }
}
