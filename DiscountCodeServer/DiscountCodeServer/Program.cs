using DiscountCodeServer.Models;
using DiscountCodeServer.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Basic Configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false);

// 2. Database
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. gRPC Services
builder.Services.AddGrpc(options => {
    options.EnableDetailedErrors = true;
    options.MaxReceiveMessageSize = 16 * 1024 * 1024; // 16MB
});

// 4. Kestrel Configuration (HTTP/2 without HTTPS)
builder.WebHost.ConfigureKestrel(options => {
    options.ListenLocalhost(5023, listenOptions => {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

var app = builder.Build();

// 5. Database Migration
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    await db.Database.MigrateAsync();
}

// 6. Endpoints
app.MapGrpcService<DiscountService>();
app.MapGet("/", () => "gRPC Server is running. Use a gRPC client to connect.");
app.MapGet("/health", () => "OK");

// 7. Startup
Console.WriteLine("Server running on http://localhost:5023");
await app.RunAsync();