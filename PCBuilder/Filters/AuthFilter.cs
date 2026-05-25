using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PCBuilder.Filters
{
    /// <summary>
    /// Globalny filtr: każdy request HTML musi być od zalogowanego użytkownika.
    /// Wyjątki: AccountController (login UI) oraz endpointy /api/auth/*.
    /// Dla wywołań API zwraca 401 JSON, dla pozostałych przekierowuje na /Account/Login.
    /// </summary>
    public class AuthFilter : IAsyncAuthorizationFilter
    {
        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var routeValues = context.RouteData.Values;
            var controller = routeValues["controller"] as string ?? "";
            var path = context.HttpContext.Request.Path.Value ?? "";

            bool isAuthApi = path.StartsWith("/api/auth", StringComparison.OrdinalIgnoreCase);
            bool isDataApi = path.StartsWith("/api/data", StringComparison.OrdinalIgnoreCase); 
            bool isAccountController = string.Equals(controller, "Account", StringComparison.OrdinalIgnoreCase);

            // DODAJEMY isDataApi DO WARUNKU:
            if (isAuthApi || isDataApi || isAccountController) return Task.CompletedTask;

            var userId = context.HttpContext.Session.GetInt32("AuthUserId");
            if (userId.HasValue) return Task.CompletedTask;

            // Niezalogowany
            bool isApi = path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase);
            if (isApi)
            {
                context.Result = new JsonResult(new { error = "Niezalogowany." })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            }
            else
            {
                var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
                context.Result = new RedirectResult($"/Account/Login?returnUrl={Uri.EscapeDataString(returnUrl)}");
            }
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Atrybut do oznaczania akcji wymagających roli Admin.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class AdminOnlyAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var role = context.HttpContext.Session.GetString("AuthRole");
            if (role != "Admin")
            {
                var path = context.HttpContext.Request.Path.Value ?? "";
                if (path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase))
                {
                    context.Result = new JsonResult(new { error = "Brak uprawnień (Admin)." })
                    {
                        StatusCode = StatusCodes.Status403Forbidden
                    };
                }
                else
                {
                    context.Result = new RedirectResult("/Home/Index");
                }
            }
        }
    }
}
