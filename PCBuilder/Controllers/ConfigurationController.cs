using Microsoft.AspNetCore.Mvc;
using PCBuilder.Data;

namespace PCBuilder.Controllers
{
    public class ConfigurationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ConfigurationController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            int? cpuId = HttpContext.Session.GetInt32("SelectedCpuId");
            int? gpuId = HttpContext.Session.GetInt32("SelectedGpuId");
            int? motherboardId = HttpContext.Session.GetInt32("SelectedMotherboardId");
            
            // --- POPRAWKA 1: Pobieranie RAM i Obudowy z sesji ---
            int? memoryId = HttpContext.Session.GetInt32("SelectedMemoryId"); // <- WAŻNE!
            int? caseId = HttpContext.Session.GetInt32("SelectedCaseId");
            int? cpuCoolerId = HttpContext.Session.GetInt32("SelectedCpuCoolerId");


            ViewBag.SelectedCpu = cpuId != null ? _context.Cpus.Find(cpuId) : null;
            ViewBag.SelectedGpu = gpuId != null ? _context.Gpus.Find(gpuId) : null;
            ViewBag.SelectedMotherboard = motherboardId != null ? _context.Motherboards.Find(motherboardId) : null;
            ViewBag.SelectedCpuCooler = cpuCoolerId != null ? _context.CpuCoolers.Find(cpuCoolerId) : null;
            // --- POPRAWKA 2: Przekazywanie obiektów do ViewBag ---
            ViewBag.SelectedMemory = memoryId != null ? _context.Memories.Find(memoryId) : null; // <- WAŻNE!
            ViewBag.SelectedCase = caseId != null ? _context.Cases.Find(caseId) : null;

            return View();
        }

        [HttpPost]
        public IActionResult AddComponent(string type, int id)
        {
            if (type == "Cpu") HttpContext.Session.SetInt32("SelectedCpuId", id);
            if (type == "Gpu") HttpContext.Session.SetInt32("SelectedGpuId", id);
            if (type == "Motherboard") HttpContext.Session.SetInt32("SelectedMotherboardId", id);
            if (type == "CpuCooler") HttpContext.Session.SetInt32("SelectedCpuCoolerId", id);
            // --- POPRAWKA 3: Obsługa zapisu RAM i Obudowy w sesji ---
            if (type == "Memory") HttpContext.Session.SetInt32("SelectedMemoryId", id); // <- WAŻNE!
            if (type == "Case") HttpContext.Session.SetInt32("SelectedCaseId", id);

            return RedirectToAction("Index");
        }

        public IActionResult RemoveComponent(string type)
        {
            if (type == "Cpu") HttpContext.Session.Remove("SelectedCpuId");
            if (type == "Gpu") HttpContext.Session.Remove("SelectedGpuId");
            if (type == "Motherboard") HttpContext.Session.Remove("SelectedMotherboardId");
            if (type == "CpuCooler") HttpContext.Session.Remove("SelectedCpuCoolerId");
            // --- POPRAWKA 4: Usuwanie pojedynczych części ---
            if (type == "Memory") HttpContext.Session.Remove("SelectedMemoryId"); // <- WAŻNE!
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