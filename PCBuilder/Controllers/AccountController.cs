using Microsoft.AspNetCore.Mvc;
using PCBuilder.Data;
using PCBuilder.Services;

namespace PCBuilder.Controllers
{

    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Login(string? returnUrl = null)
        {
            if (HttpContext.Session.GetInt32("AuthUserId").HasValue)
            {
                return Redirect(string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl);
            }

            ViewBag.ReturnUrl = returnUrl ?? "/";
            return View();
        }
        public IActionResult Logout()
        {
            var userId = HttpContext.Session.GetInt32("AuthUserId");
            if (userId.HasValue)
            {
                ConfigurationStore.PersistSessionToUser(HttpContext.Session, _context, userId.Value);
            }
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }
    }
}
