using fourHorsemen_Online_Video_Game_Database.Data;
using fourHorsemen_Online_Video_Game_Database.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Configuration;
using fourHorsemen_Online_Video_Game_Database.Models;


var builder = WebApplication.CreateBuilder(args);

// Configure EmailSettings from appsettings.json
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

//registers the service so it can be injected into controllers
builder.Services.AddHttpClient<RawgApiService>();

builder.Services.AddSingleton<FunFactsService>();

builder.Services.AddSingleton<GameNewsService>();


AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(Directory.GetCurrentDirectory(), "Data"));


builder.Services.AddDbContext<GameDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));




// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<GameDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>() //adding Roles
.AddEntityFrameworkStores<GameDBContext>();

builder.Services.AddTransient<IEmailSender, EmailSender>();

// Register the DbContext with the connection string
builder.Services.AddDbContext<GameDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



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

app.MapRazorPages();
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "privacy",
    pattern: "Privacy",
    defaults: new { controller = "Home", action = "Privacy" });

app.MapControllerRoute(
    name: "history",
    pattern: "History",
    defaults: new { controller = "Home", action = "History" });

app.MapControllerRoute(
    name: "facts",
    pattern: "Facts",
    defaults: new { controller = "Home", action = "Facts" });

app.Run();
