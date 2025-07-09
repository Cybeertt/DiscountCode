using DiscountCodeServer.Models;
using DiscountCodeServer.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Add DbContext with SQLite provider
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Add other services
builder.Services.AddGrpc();

var app = builder.Build();

// 3. Apply migrations - PROPER IMPLEMENTATION
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

    try
    {
        // Will create database if doesn't exist and apply all migrations
        dbContext.Database.Migrate();
        app.Logger.LogInformation("Database migrated successfully");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "An error occurred while migrating the database");
        throw; // Fail fast if migrations fail
    }
}

// 4. Configure endpoints
app.MapGrpcService<DiscountService>();
app.MapGet("/", () => "gRPC server is running. Use a gRPC client to connect.");

app.Run();