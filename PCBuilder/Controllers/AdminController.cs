using Microsoft.AspNetCore.Mvc;
using PCBuilder.Filters;


namespace PCBuilder.Controllers
{
    [AdminOnly]
    public class AdminController : Controller
    {
        public IActionResult Users()
        {
            return View();
        }
    }
}
