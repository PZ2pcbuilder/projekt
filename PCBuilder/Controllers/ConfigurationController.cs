using Microsoft.AspNetCore.Mvc;
using PCBuilder.Data;
using PCBuilder.Services;


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
            int? memoryId = HttpContext.Session.GetInt32("SelectedMemoryId");
            int? caseId = HttpContext.Session.GetInt32("SelectedCaseId");
            int? cpuCoolerId = HttpContext.Session.GetInt32("SelectedCpuCoolerId");
            int? storageId = HttpContext.Session.GetInt32("SelectedStorageId");
            int? psuId = HttpContext.Session.GetInt32("SelectedPsuId");

            var cpu = cpuId != null ? _context.Cpus.Find(cpuId) : null;
            var gpu = gpuId != null ? _context.Gpus.Find(gpuId) : null;
            ViewBag.SelectedStorage = storageId != null ? _context.Storages.Find(storageId) : null;
            ViewBag.SelectedCpu = cpuId != null ? _context.Cpus.Find(cpuId) : null;
            ViewBag.SelectedGpu = gpuId != null ? _context.Gpus.Find(gpuId) : null;
            ViewBag.SelectedMotherboard = motherboardId != null ? _context.Motherboards.Find(motherboardId) : null;
            ViewBag.SelectedCpuCooler = cpuCoolerId != null ? _context.CpuCoolers.Find(cpuCoolerId) : null;
            ViewBag.SelectedMemory = memoryId != null ? _context.Memories.Find(memoryId) : null;
            ViewBag.SelectedCase = caseId != null ? _context.Cases.Find(caseId) : null;
            ViewBag.SelectedPowerSupply = psuId != null ? _context.PowerSupplies.Find(psuId) : null;

            int tdpSuma = 0;
            if (cpu != null) tdpSuma += cpu.Tdp;
            if (gpu != null) tdpSuma += gpu.RecommendedPsuW;

            if (tdpSuma > 0)
            {
                tdpSuma += 70;
            }

            ViewBag.TotalTdp = tdpSuma;
            ViewBag.RecommendedWattage = (int)Math.Ceiling(tdpSuma * 1.3 / 50.0) * 50;
            return View();
        }

        [HttpPost]
        public IActionResult AddComponent(string type, int id)
        {
            if (type == "Cpu") HttpContext.Session.SetInt32("SelectedCpuId", id);
            if (type == "Gpu") HttpContext.Session.SetInt32("SelectedGpuId", id);
            if (type == "Motherboard") HttpContext.Session.SetInt32("SelectedMotherboardId", id);
            if (type == "CpuCooler") HttpContext.Session.SetInt32("SelectedCpuCoolerId", id);
            if (type == "Memory") HttpContext.Session.SetInt32("SelectedMemoryId", id);
            if (type == "Case") HttpContext.Session.SetInt32("SelectedCaseId", id);
            if (type == "Storage") HttpContext.Session.SetInt32("SelectedStorageId", id);
            if (type == "Psu" || type == "PowerSupply") HttpContext.Session.SetInt32("SelectedPsuId", id);

            PersistIfLoggedIn();
            return RedirectToAction("Index");
        }

        public IActionResult RemoveComponent(string type)
        {
            if (type == "Cpu") HttpContext.Session.Remove("SelectedCpuId");
            if (type == "Gpu") HttpContext.Session.Remove("SelectedGpuId");
            if (type == "Motherboard") HttpContext.Session.Remove("SelectedMotherboardId");
            if (type == "CpuCooler") HttpContext.Session.Remove("SelectedCpuCoolerId");
            if (type == "Memory") HttpContext.Session.Remove("SelectedMemoryId");
            if (type == "Case") HttpContext.Session.Remove("SelectedCaseId");
            if (type == "Storage") HttpContext.Session.Remove("SelectedStorageId");
            if (type == "Psu" || type == "PowerSupply") HttpContext.Session.Remove("SelectedPsuId");

            PersistIfLoggedIn();
            return RedirectToAction("Index");
        }

        public IActionResult Clear()
        {
            foreach (var key in ConfigurationStore.ComponentKeys)
            {
                HttpContext.Session.Remove(key);
            }
            PersistIfLoggedIn();
            return RedirectToAction("Index");
        }

        private void PersistIfLoggedIn()
        {
            var userId = HttpContext.Session.GetInt32("AuthUserId");
            if (userId.HasValue)
            {
                ConfigurationStore.PersistSessionToUser(HttpContext.Session, _context, userId.Value);
            }
        }
    }
}
