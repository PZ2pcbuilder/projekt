using Microsoft.EntityFrameworkCore;
using PCBuilder.Data;

var builder = WebApplication.CreateBuilder(args);

// ==========================================================
// 1. REJESTRACJA USŁUG (Wszystko z builder.Services)
// ==========================================================

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();

// TUTAJ PRZENIESIONE: Rejestracja pamięci podręcznej i sesji przed zbudowaniem aplikacji
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Koszyk wygasa po 30 min bezruchu
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// ==========================================================
// ZBUDOWANIE APLIKACJI (Od tego momentu usługi są TYLKO DO ODCZYTU)
// ==========================================================
var app = builder.Build();


// ==========================================================
// 2. INICJALIZACJA BAZY DANYCH (Wykorzystanie zbudowanych usług)
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
// 3. KONFIGURACJA POTOKU ŻĄDAŃ HTTP (Middleware - app.Use...)
// ==========================================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Włączenie obsługi sesji w potoku (pomiędzy Routingiem a Autoryzacją)
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
