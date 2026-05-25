using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCBuilder.Data;
using PCBuilder.Models;
using PCBuilder.Services;

namespace PCBuilder.Controllers.Api
{
    /// <summary>
    /// REST API uwierzytelniania (logowanie, rejestracja, wylogowanie, panel admina).
    /// Wszystkie odpowiedzi są w formacie JSON.
    /// Stan logowania trzymany jest w sesji (HttpContext.Session) – cookie ASP.NET Core.
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    public class AuthApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        public record LoginRequest(string Username, string Password);
        public record RegisterRequest(string Username, string Password);
        public record ChangeRoleRequest(string Role);
        public record ChangePasswordRequest(string NewPassword);

        // ===================== PUBLICZNE =====================

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest(new { error = "Brak loginu lub hasła." });

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == req.Username);
            if (user == null || !VerifyPassword(req.Password, user.PasswordHash))
                return Unauthorized(new { error = "Nieprawidłowy login lub hasło." });

            if (!user.PasswordHash.StartsWith("$2"))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password);
                await _context.SaveChangesAsync();
            }

            HttpContext.Session.SetInt32("AuthUserId", user.Id);
            HttpContext.Session.SetString("AuthUsername", user.Username);
            HttpContext.Session.SetString("AuthRole", user.Role);

            // Przywróć zapisaną konfigurację z poprzedniej sesji
            ConfigurationStore.RestoreToSession(HttpContext.Session, user.SavedConfigJson);

            return Ok(new
            {
                id = user.Id,
                username = user.Username,
                role = user.Role,
                token = user.ApiToken
            });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            var userId = HttpContext.Session.GetInt32("AuthUserId");
            if (userId.HasValue)
            {
                // Zapisz konfigurację przed wyczyszczeniem sesji
                ConfigurationStore.PersistSessionToUser(HttpContext.Session, _context, userId.Value);
            }
            HttpContext.Session.Clear();
            return Ok(new { ok = true });
        }

        [HttpGet("me")]
        public IActionResult Me()
        {
            var userId = HttpContext.Session.GetInt32("AuthUserId");
            if (!userId.HasValue) return Unauthorized(new { error = "Niezalogowany." });

            var user = _context.Users.Find(userId.Value);
            if (user == null)
            {
                HttpContext.Session.Clear();
                return Unauthorized(new { error = "Sesja wygasła." });
            }

            return Ok(new
            {
                id = user.Id,
                username = user.Username,
                role = user.Role
            });
        }

        // ===================== ADMIN: ZARZĄDZANIE UŻYTKOWNIKAMI =====================

        [HttpGet("users")]
        public IActionResult ListUsers()
        {
            if (!IsAdmin()) return Forbid();
            var users = _context.Users
                .OrderBy(u => u.Id)
                .Select(u => new { u.Id, u.Username, u.Role })
                .ToList();
            return Ok(users);
        }

        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] RegisterRequest req)
        {
            if (!IsAdmin()) return Forbid();
            if (req == null || string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest(new { error = "Wymagany login i hasło." });
            if (req.Password.Length < 4)
                return BadRequest(new { error = "Hasło musi mieć przynajmniej 4 znaki." });

            if (await _context.Users.AnyAsync(u => u.Username == req.Username))
                return Conflict(new { error = "Użytkownik o takiej nazwie już istnieje." });

            var user = new User
            {
                Username = req.Username,
                Role = "User",
                ApiToken = Guid.NewGuid().ToString("N"),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password)
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { user.Id, user.Username, user.Role });
        }

        [HttpDelete("users/{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (!IsAdmin()) return Forbid();
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            if (user.Username == "admin")
                return BadRequest(new { error = "Nie można usunąć konta administratora." });

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok(new { ok = true });
        }

        [HttpPut("users/{id:int}/role")]
        public async Task<IActionResult> ChangeRole(int id, [FromBody] ChangeRoleRequest req)
        {
            if (!IsAdmin()) return Forbid();
            if (req == null || string.IsNullOrWhiteSpace(req.Role)) return BadRequest();
            if (req.Role != "Admin" && req.Role != "User")
                return BadRequest(new { error = "Rola musi być 'Admin' lub 'User'." });

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            if (user.Username == "admin" && req.Role != "Admin")
                return BadRequest(new { error = "Konto admin musi pozostać administratorem." });

            user.Role = req.Role;
            await _context.SaveChangesAsync();
            return Ok(new { user.Id, user.Username, user.Role });
        }

        [HttpPut("users/{id:int}/password")]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest req)
        {
            if (!IsAdmin()) return Forbid();
            if (req == null || string.IsNullOrWhiteSpace(req.NewPassword) || req.NewPassword.Length < 4)
                return BadRequest(new { error = "Hasło musi mieć minimum 4 znaki." });

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);
            await _context.SaveChangesAsync();
            return Ok(new { ok = true });
        }

        // ===================== HELPERY =====================

        private bool IsAdmin()
            => HttpContext.Session.GetString("AuthRole") == "Admin";

        private static bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrEmpty(hash)) return false;
            if (hash.StartsWith("$2"))
            {
                try { return BCrypt.Net.BCrypt.Verify(password, hash); }
                catch { return false; }
            }
            // Fallback: stary format Identity (PasswordHasher) – baza w Base64
            try
            {
                var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
                var stub = new User { Username = "", PasswordHash = hash, Role = "", ApiToken = "" };
                var result = hasher.VerifyHashedPassword(stub, hash, password);
                return result != Microsoft.AspNetCore.Identity.PasswordVerificationResult.Failed;
            }
            catch
            {
                return false;
            }
        }
    }
}
