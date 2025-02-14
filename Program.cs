
using Microsoft.AspNetCore.Identity;
using WebApplication1.Middleware;
using WebApplication1.Model;
using WebApplication1.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDbContext<ApplicationDbContext>();
// singleton for DI
builder.Services.AddSingleton<AesEncryptionService, AesEncryptionService>();
builder.Services.AddScoped<AuditService, AuditService>();
builder.Services.AddScoped<SessionService, SessionService>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 12;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = true;

    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDistributedMemoryCache(); //save session in memory
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.IdleTimeout = TimeSpan.FromMinutes(1);
    options.Cookie.SameSite = SameSiteMode.Strict;
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication("MyCookieAuth").AddCookie("MyCookieAuth", options
=>
{
	options.Cookie.Name = "MyCookieAuth";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MustBelongToHRDepartment",
    policy => policy.RequireClaim("Department", "HR"));
});

builder.Services.ConfigureApplicationCookie(Config =>
{
    Config.LoginPath = "/Login";
}
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession();
app.UseMiddleware<SessionValidationMiddleware>();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseStatusCodePages(async context =>
{
    var response = context.HttpContext.Response;
    var requestPath = context.HttpContext.Request.Path.Value;
    var statusCode = response.StatusCode;

    // Check if we're already on an error page
    if (requestPath.StartsWith("/404") ||
        requestPath.StartsWith("/500") ||
        requestPath.StartsWith("/401"))
    {
        return;
    }

    switch (statusCode)
    {
        case 401:
            response.Redirect("/401");
            break;
        case 404:
            response.Redirect("/404");
            break;
        case 500:
            response.Redirect("/500");
            break;
        default:
            response.Redirect("/404");
            break;
    }

    await Task.CompletedTask;
});
app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
