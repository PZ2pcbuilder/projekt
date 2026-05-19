using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCBuilder.Data;
using PCBuilder.Models.Filters;

namespace PCBuilder.Controllers
{
    public class StorageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StorageController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(StorageFilter filter)
        {
            filter ??= new StorageFilter();
            var query = _context.Storages.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var s = filter.Search.ToLower();
                query = query.Where(x =>
                    x.Name.ToLower().Contains(s) ||
                    x.Type.ToLower().Contains(s) ||
                    x.Interface.ToLower().Contains(s) ||
                    x.FormFactor.ToLower().Contains(s));
            }

            if (filter.MinPrice.HasValue) query = query.Where(x => x.Price >= filter.MinPrice);
            if (filter.MaxPrice.HasValue) query = query.Where(x => x.Price <= filter.MaxPrice);
            if (filter.MinCapacity.HasValue) query = query.Where(x => x.Capacity >= filter.MinCapacity);
            if (filter.MaxCapacity.HasValue) query = query.Where(x => x.Capacity <= filter.MaxCapacity);
            if (filter.MinCache.HasValue) query = query.Where(x => x.Cache >= filter.MinCache);
            if (filter.MaxCache.HasValue) query = query.Where(x => x.Cache <= filter.MaxCache);
            if (!string.IsNullOrWhiteSpace(filter.Type)) query = query.Where(x => x.Type == filter.Type);
            if (!string.IsNullOrWhiteSpace(filter.FormFactor)) query = query.Where(x => x.FormFactor == filter.FormFactor);
            if (!string.IsNullOrWhiteSpace(filter.Interface)) query = query.Where(x => x.Interface == filter.Interface);

            var types = await _context.Storages.Select(x => x.Type).Distinct().OrderBy(s => s).ToListAsync();
            var formFactors = await _context.Storages.Select(x => x.FormFactor).Distinct().OrderBy(s => s).ToListAsync();
            var interfaces = await _context.Storages.Select(x => x.Interface).Distinct().OrderBy(s => s).ToListAsync();

            var vm = new StorageIndexViewModel
            {
                Items = await query.OrderBy(x => x.Type).ThenBy(x => x.Name).ToListAsync(),
                Filter = filter,
                Types = types,
                FormFactors = formFactors,
                Interfaces = interfaces
            };
            return View(vm);
        }
    }
}
