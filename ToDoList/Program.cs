using DataAccess;
using DataAccess.Context;
using Business.Interfaces;
using Business.Managers;
using DataAccess.Interfaces;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// 1️⃣ Servis Katmanı Bağımlılıkları (Business → DataAccess)
builder.Services.AddScoped<IUserService, UserManager>();
builder.Services.AddScoped<IUserRepository, EfUserRepository>();

builder.Services.AddScoped<IToDoService, ToDoManager>();
builder.Services.AddScoped<IToDoRepository, EfToDoRepository>();

// 2️⃣ Veritabanı Bağlantısı (DbContext)
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// 3️⃣ Cookie Authentication Ayarları
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/Account/Login";          // Giriş yapılmamışsa yönlendirme
        options.LogoutPath = "/Account/Logout";        // Çıkış
        options.AccessDeniedPath = "/Account/Login";   // Gerekirse yetkisiz erişim
    });

// 4️⃣ MVC için controller ve view servisi
builder.Services.AddControllersWithViews();

// 5️⃣ Uygulamayı oluştur
var app = builder.Build();


// 6️⃣ Ortam kontrolleri
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// 7️⃣ Ortak middleware ayarları
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// 8️⃣ Authentication ve Authorization middleware
app.UseAuthentication(); // Giriş işlemleri için
app.UseAuthorization();  // Roller vs. için (şu an kullanılmıyor ama hazır dursun)

// 9️⃣ Varsayılan route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=ToDo}/{action=Index}/{id?}");

// 10️⃣ Uygulamayı çalıştır
app.Run();
