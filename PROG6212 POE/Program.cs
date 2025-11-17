using PROG6212_POE.Services;
using PROG6212_POE.Models.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    });

// Register services with enhanced logging
builder.Services.AddScoped<IClaimService, ClaimService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddLogging();

// Add background service for automated tasks
builder.Services.AddHostedService<ClaimAutomationService>();

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
app.UseAuthentication();
app.UseAuthorization();

// Enhanced auto-set default user session for demo purposes
app.Use(async (context, next) =>
{
    // If no user is logged in, automatically set as lecturer1
    if (context.Session.GetInt32("UserId") == null)
    {
        context.Session.SetInt32("UserId", 1); // lecturer1
        context.Session.SetInt32("UserRole", (int)UserType.Lecturer);
        context.Session.SetString("UserName", "Dr. John Smith (Demo)");
        context.Session.SetString("UserEmail", "lecturer1@university.com");
    }
    await next();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Dashboard}/{id?}");

app.Run();

// Background service for automated claim processing
public class ClaimAutomationService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ClaimAutomationService> _logger;

    public ClaimAutomationService(IServiceProvider serviceProvider, ILogger<ClaimAutomationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var claimService = scope.ServiceProvider.GetRequiredService<IClaimService>();

                // Auto-approve eligible claims every hour
                await claimService.AutoApproveClaimsAsync();

                _logger.LogInformation("Automated claim processing completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in automated claim processing");
            }

            // Run every hour
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}