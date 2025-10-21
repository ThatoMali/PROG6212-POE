using PROG6212_POE.Services;
using PROG6212_POE.Models.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register services
builder.Services.AddScoped<IClaimService, ClaimService>();
builder.Services.AddScoped<IFileService, FileService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();

// Auto-set default user session (Lecturer) for demo purposes
app.Use(async (context, next) =>
{
    // If no user is logged in, automatically set as lecturer1
    if (context.Session.GetInt32("UserId") == null)
    {
        context.Session.SetInt32("UserId", 1); // lecturer1
        context.Session.SetInt32("UserRole", (int)UserType.Lecturer);
        context.Session.SetString("UserName", "Dr. John Smith (Demo)");
    }
    await next();
});

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Dashboard}/{id?}");

app.Run();