using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCBuilder.Data;
using PCBuilder.Models.Filters;


namespace PCBuilder.Controllers
{
    public class CpuController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CpuController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(CpuFilter filter)
        {
            filter ??= new CpuFilter();
            var query = _context.Cpus.AsQueryable();

            
            int? selectedMotherboardId = HttpContext.Session.GetInt32("SelectedMotherboardId");
            if (selectedMotherboardId != null)
            {
                var selectedMotherboard = await _context.Motherboards.FindAsync(selectedMotherboardId);
                if (selectedMotherboard != null)
                {
                    query = query.Where(c => c.Socket == selectedMotherboard.Socket);
                    ViewData["CompatibilityMessage"] = $"Pokazuję procesory pasujące do płyty z gniazdem {selectedMotherboard.Socket}";
                }
            }


            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var s = filter.Search.ToLower();
                query = query.Where(c =>
                    (c.Name != null && c.Name.ToLower().Contains(s)) ||
                    (c.Socket != null && c.Socket.ToLower().Contains(s)) ||
                    (c.Microarchitecture != null && c.Microarchitecture.ToLower().Contains(s)));
            }


            if (filter.MinPrice.HasValue) query = query.Where(c => c.Price >= filter.MinPrice);
            if (filter.MaxPrice.HasValue) query = query.Where(c => c.Price <= filter.MaxPrice);
            if (filter.MinCores.HasValue) query = query.Where(c => c.CoreCount >= filter.MinCores);
            if (filter.MaxCores.HasValue) query = query.Where(c => c.CoreCount <= filter.MaxCores);
            if (filter.MinTdp.HasValue) query = query.Where(c => c.Tdp >= filter.MinTdp);
            if (filter.MaxTdp.HasValue) query = query.Where(c => c.Tdp <= filter.MaxTdp);
            if (!string.IsNullOrWhiteSpace(filter.Socket)) query = query.Where(c => c.Socket == filter.Socket);
            if (!string.IsNullOrWhiteSpace(filter.MemoryType)) query = query.Where(c => c.MemoryType == filter.MemoryType);
            if (!string.IsNullOrWhiteSpace(filter.Microarchitecture)) query = query.Where(c => c.Microarchitecture == filter.Microarchitecture);

            // Listy do dropdownów: zawsze z całej bazy (nie z przefiltrowanego zbioru)
            var sockets = await _context.Cpus.Select(c => c.Socket).Where(s => s != null).Distinct().OrderBy(s => s).ToListAsync();
            var memTypes = await _context.Cpus.Select(c => c.MemoryType).Where(s => s != null).Distinct().OrderBy(s => s).ToListAsync();
            var archs = await _context.Cpus.Select(c => c.Microarchitecture).Where(s => s != null).Distinct().OrderBy(s => s).ToListAsync();

            var vm = new CpuIndexViewModel
            {
                Items = await query.OrderBy(c => c.Name).ToListAsync(),
                Filter = filter,
                Sockets = sockets!,
                MemoryTypes = memTypes!,
                Microarchitectures = archs!
            };
            return View(vm);
        }
    }
}
