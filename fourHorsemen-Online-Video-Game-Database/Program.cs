using fourHorsemen_Online_Video_Game_Database.Data;
using fourHorsemen_Online_Video_Game_Database.Services;
using Microsoft.EntityFrameworkCore;



var builder = WebApplication.CreateBuilder(args);

//registers the service so it can be injected into controllers
builder.Services.AddHttpClient<RawgApiService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

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

app.UseAuthorization();

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
