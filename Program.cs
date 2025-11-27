using Microsoft.EntityFrameworkCore;
using PaytrackR.Components;
using PaytrackR.Data;
using PaytrackR.Services;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add business services
builder.Services.AddScoped<ShiftService>();

// Add health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// **AUTO-MIGRATE DATABASE ON STARTUP**
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        logger.LogInformation("Starting database migration...");
        
        // Apply any pending migrations
        context.Database.Migrate();
        
        logger.LogInformation("Database migration completed successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while migrating the database");
        throw; // Prevent app from starting if database is not ready
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.UseHttpMetrics();
app.MapMetrics();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Map health check endpoint
app.MapHealthChecks("/health");

app.Run();
