using AdminTemplate.Data;
using AdminTemplate.Repositories;
using AdminTemplate.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();