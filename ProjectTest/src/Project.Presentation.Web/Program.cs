using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Project.DataAccess;
using Project.DataAccess.Models;
using Project.Presentation.Web.Configurations;
using Project.Presentation.Web.Models;
using Project.Presentation.Web.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

IServiceCollection services = builder.Services;

//Dependencies.ConfigureServices(builder.Configuration, services);

// Tambahkan Connection String langsung di sini
services.AddDbContext<ProjectContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TestConnection"))
);

services.AddIdentity<Users, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ProjectContext>()
    .AddDefaultTokenProviders();

services.Configure<IdentityOptions>(options =>
{
    // Jumlah percobaan login gagal sebelum lockout
    options.Lockout.MaxFailedAccessAttempts = 5;

    // Berapa lama akun dikunci (misalnya 5 menit)
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

    // Apakah fitur lockout diaktifkan
    options.Lockout.AllowedForNewUsers = true;
});

services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    });




services.AddBussinesServices();

services.AddScoped<RequestBarangService>();
services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
services.AddTransient<EmailService>();


builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});



var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
