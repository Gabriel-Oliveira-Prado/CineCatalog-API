using Serilog;
using CineCatalog_API.Extensions;
using CineCatalog_API.Middlewares;
using CineCatalog_API.Infrastructure.Data.Seed;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog from appsettings.json
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configure Custom Dependency Injection (Database, Repositories, Services, JWT, AutoMapper, FluentValidation)
builder.Services.AddApplicationServices(builder.Configuration);

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocumentation();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CineCatalog API v1");
    });
}

// Global Exception Handler Middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseCors("DefaultPolicy");

// Authentication & Authorization (Critical order!)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Apply database migrations and seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await DbInitializer.InitializeAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro ao migrar ou semear o banco de dados.");
    }
}

try
{
    Log.Information("Iniciando CineCatalog API...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "A CineCatalog API falhou ao iniciar.");
}
finally
{
    Log.CloseAndFlush();
}
