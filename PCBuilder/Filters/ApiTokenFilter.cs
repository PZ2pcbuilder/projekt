using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using PCBuilder.Data;

namespace PCBuilder.Filters
{
    public class ApiTokenFilter : IAsyncAuthorizationFilter
    {
        private readonly ApplicationDbContext _context;

        public ApiTokenFilter(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var request = context.HttpContext.Request;

            // Sprawdzamy, czy w nagłówkach są przesłane login i token
            if (!request.Headers.TryGetValue("X-Username", out var usernameValues) ||
                !request.Headers.TryGetValue("X-Api-Token", out var tokenValues))
            {
                context.Result = new JsonResult(new { error = "Brak nagłówków autoryzacji REST (X-Username, X-Api-Token)." })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return;
            }

            string username = usernameValues.ToString();
            string token = tokenValues.ToString();

            // Weryfikacja z bazą danych
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username && u.ApiToken == token);
            if (user == null)
            {
                context.Result = new JsonResult(new { error = "Błędny login lub token." })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return;
            }

            // Opcjonalne: zapisanie usera w kontekście żądania
            context.HttpContext.Items["ValidatedUser"] = user;
        }
    }
}