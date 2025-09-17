using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WebTask.Entities;
using WebTask.Interfaces;
using WebTask.IServices;
using WebTask.Mappings;
using WebTask.Repositories;
using WebTask.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ---------------------- Services Configuration ---------------------- //

// Controllers + JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Enum as string
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

        // Keep PascalCase (optional: set to null if you don’t want camelCase)
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        // Custom model validation handling (instead of automatic 400)
        options.SuppressModelStateInvalidFilter = true;
    });

// AutoMapper
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});
// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()     // Allow requests from any domain (you can restrict to your Angular URL later)
            .AllowAnyMethod()     // Allow GET, POST, PUT, DELETE, etc.
            .AllowAnyHeader();    // Allow all headers
    });
});

// Dependency Injection
builder.Services.AddScoped<ITasksRepository, TasksRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITasksService, TasksService>();

// EF Core with SQL Server
builder.Services.AddDbContext<DbTaskContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "WebTask API",
        Version = "v1",
        Description = "API for managing tasks with UnitOfWork & Repository pattern",
        Contact = new OpenApiContact
        {
            Name = "Mahmoud Ammar",
            Email = "ammarmahmoud1996@gmail.com"
        }
    });

   
    c.SchemaGeneratorOptions = new Swashbuckle.AspNetCore.SwaggerGen.SchemaGeneratorOptions
    {
        SchemaFilters = { }
    };

    c.UseInlineDefinitionsForEnums();
});


// ---------------------- Middleware Pipeline ---------------------- //

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebTask API v1");
        c.RoutePrefix = string.Empty; // Swagger at root "/"
    });
}
// Use CORS
app.UseCors("AllowAll");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
