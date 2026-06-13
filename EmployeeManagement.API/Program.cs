using EmployeeManagement.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add application services (DbContext, Identity, JWT, Swagger, etc.)
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Employee Management API V1");
    });
}

app.UseHttpsRedirection();

// Use custom middleware configuration (CORS, Auth, etc.)
app.UseApplicationMiddleware();

app.MapControllers();

app.Run();
