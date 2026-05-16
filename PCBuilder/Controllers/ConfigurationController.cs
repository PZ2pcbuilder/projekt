using Microsoft.AspNetCore.Mvc;
using PCBuilder.Data;

namespace PCBuilder.Controllers
{
    public class ConfigurationController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Wstrzykujemy bazę danych, aby móc wyciągnąć nazwy po ID
        public ConfigurationController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            int? cpuId = HttpContext.Session.GetInt32("SelectedCpuId");
            int? gpuId = HttpContext.Session.GetInt32("SelectedGpuId");
            int? motherboardId = HttpContext.Session.GetInt32("SelectedMotherboardId");

            // Szukamy w bazie pełnych obiektów. Jeśli ID nie ma w sesji, Find() zwróci null.
            ViewBag.SelectedCpu = cpuId != null ? _context.Cpus.Find(cpuId) : null;
            ViewBag.SelectedGpu = gpuId != null ? _context.Gpus.Find(gpuId) : null;
            ViewBag.SelectedMotherboard = motherboardId != null ? _context.Motherboards.Find(motherboardId) : null;
            int? caseId = HttpContext.Session.GetInt32("SelectedCaseId");
            ViewBag.SelectedCase = caseId != null ? _context.Cases.Find(caseId) : null;
            return View();
        }

        [HttpPost]
        public IActionResult AddComponent(string type, int id)
        {
            if (type == "Cpu") HttpContext.Session.SetInt32("SelectedCpuId", id);
            if (type == "Gpu") HttpContext.Session.SetInt32("SelectedGpuId", id);
            if (type == "Motherboard") HttpContext.Session.SetInt32("SelectedMotherboardId", id);
            if (type == "Case") HttpContext.Session.SetInt32("SelectedCaseId", id);

            return RedirectToAction("Index");
        }

        public IActionResult RemoveComponent(string type)
        {
            if (type == "Cpu") HttpContext.Session.Remove("SelectedCpuId");
            if (type == "Gpu") HttpContext.Session.Remove("SelectedGpuId");
            if (type == "Motherboard") HttpContext.Session.Remove("SelectedMotherboardId");
            if (type == "Case") HttpContext.Session.Remove("SelectedCaseId");
            
            return RedirectToAction("Index");
        }

        public IActionResult Clear()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}