using AdminTemplate.Configuration;
using AdminTemplate.Data;
using AdminTemplate.Filters;
using AdminTemplate.Repositories;
using AdminTemplate.Services;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ Register GoogleMapsSettings
builder.Services.Configure<GoogleMapsSettings>(
    builder.Configuration.GetSection("GoogleMaps"));

// REPOSITORIES
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<IInventoryCategoryRepository, InventoryCategoryRepository>();

// QUEUE SERVICES (Singleton for background processing)
builder.Services.AddSingleton<SupplierQueueService>();
builder.Services.AddSingleton<ISupplierQueueService>(sp =>
    sp.GetRequiredService<SupplierQueueService>());
builder.Services.AddHostedService(sp =>
    sp.GetRequiredService<SupplierQueueService>());

// MESSAGE QUEUE SERVICES
builder.Services.AddSingleton<IMessageQueueService, RabbitMQService>();
builder.Services.AddHostedService<RabbitMQConsumerService>();

// APPLICATION SERVICES
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IInventoryCategoryService, InventoryCategoryService>();
builder.Services.AddScoped<IFileService, FileService>();

// ✅ EMAIL SERVICE (REQUIRED FOR HANGFIRE NOTIFICATIONS)
builder.Services.AddScoped<IEmailService, EmailService>();

// HANGFIRE BACKGROUND JOB SERVICES
builder.Services.AddScoped<IInventoryMonitoringService, InventoryMonitoringService>();

// Configure Hangfire
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"),
        new SqlServerStorageOptions
        {
            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            QueuePollInterval = TimeSpan.Zero,
            UseRecommendedIsolationLevel = true,
            DisableGlobalLocks = true
        }));

// Add Hangfire server
builder.Services.AddHangfireServer();

// Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Signin/Index";
        options.LogoutPath = "/Signin/Logout";
        options.AccessDeniedPath = "/Signin/Index";
    });

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Authentication & Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Hangfire Dashboard (must be after UseAuthorization)
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() }
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Initialize recurring jobs on startup
using (var scope = app.Services.CreateScope())
{
    var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

    // Check inventory levels every 30 minutes
    recurringJobManager.AddOrUpdate<IInventoryMonitoringService>(
        "check-inventory-levels",
        service => service.CheckInventoryLevelsAsync(),
        "*/30 * * * *");
}

app.Run();