using Microsoft.EntityFrameworkCore;
using PCBuilder.Data;
using PCBuilder.Filters;

var builder = WebApplication.CreateBuilder(args);

// ==========================================================
// 1. REJESTRACJA USŁUG
// ==========================================================

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Globalny filtr autoryzacji – każde żądanie HTML wymaga zalogowanego użytkownika
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<AuthFilter>();
});
builder.Services.AddScoped<PCBuilder.Filters.ApiTokenFilter>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".PCBuilder.Session";
});

// HttpContext dostępny w klasach pomocniczych (np. do zapisu konfiguracji)
builder.Services.AddHttpContextAccessor();

var app = builder.Build();


// ==========================================================
// 2. INICJALIZACJA BAZY DANYCH
// ==========================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        DbInitializer.Initialize(context);
        Console.WriteLine(">>> Sukces: Baza danych zainicjowana!");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Błąd podczas inicjalizacji bazy danych!");
    }
}


// ==========================================================
// 3. KONFIGURACJA POTOKU ŻĄDAŃ HTTP
// ==========================================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
