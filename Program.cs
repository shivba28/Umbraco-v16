using UmbracoV16.Core.Middleware;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddComposers()
    .Build();

// Add MVC services for custom controllers
builder.Services.AddControllersWithViews();

WebApplication app = builder.Build();

await app.BootUmbracoAsync();

// Add custom logout redirect middleware
app.UseMiddleware<LogoutRedirectMiddleware>();

app.UseUmbraco()
    .WithMiddleware(u =>
    {
        u.UseBackOffice();
        u.UseWebsite();
    })
    .WithEndpoints(u =>
    {
        u.UseBackOfficeEndpoints();
        u.UseWebsiteEndpoints();
    });

// Configure custom controller routes
app.MapControllerRoute(
    name: "admin",
    pattern: "admin",
    defaults: new { controller = "Admin", action = "Index" });

app.MapControllerRoute(
    name: "customLogin",
    pattern: "login",
    defaults: new { controller = "CustomBackOfficeLogin", action = "Login" });

app.MapControllerRoute(
    name: "customLogout",
    pattern: "logout",
    defaults: new { controller = "Logout", action = "Index" });

// Add a catch-all route for our custom controllers
app.MapControllerRoute(
    name: "customControllers",
    pattern: "{controller}/{action=Index}/{id?}");

await app.RunAsync();
