using EmployeeManagement.API.Extensions;
using EmployeeManagement.API.Filters;
using EmployeeManagement.API.Middleware;
using EmployeeManagement.Application.Mapping;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Infrastructure.Seeding;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});
builder.Services.AddEndpointsApiExplorer();

// Add application services (DbContext, Identity, JWT, Swagger, etc.)
builder.Services.AddApplicationServices(builder.Configuration);

// Register DataSeeder
builder.Services.AddScoped<DataSeeder>();

// Configure Mapster
MappingConfig.RegisterMaps();

var app = builder.Build();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    await seeder.SeedAsync();
}

// Configure the HTTP request pipeline
// Global exception handler must be first
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Employee Management API V1");
});

app.UseHttpsRedirection();

// Use custom middleware configuration (CORS, Auth, etc.)
app.UseApplicationMiddleware();

app.MapControllers();

app.Run();
