using Microsoft.AspNetCore.Mvc;
using PCBuilder.Data;
using PCBuilder.Services;

namespace PCBuilder.Controllers
{
    /// <summary>
    /// Strony HTML powiązane z logowaniem. Operacje (login/logout) wykonywane są
    /// klientowsko przez fetch() do REST API /api/auth/*.
    /// </summary>
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Login(string? returnUrl = null)
        {
            // Już zalogowany?  Idziemy dalej.
            if (HttpContext.Session.GetInt32("AuthUserId").HasValue)
            {
                return Redirect(string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl);
            }

            ViewBag.ReturnUrl = returnUrl ?? "/";
            return View();
        }

        // GET /Account/Logout – odporne na proste linki w nav
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
