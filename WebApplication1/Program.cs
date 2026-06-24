using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.OpenApi;
using WebApplication1.Services.Application.CommandServices;
using WebApplication1.Services.Application.Internal.CommandServices;
using WebApplication1.Services.Domain.Repositories;
using WebApplication1.Services.Infrastructure.Persistence.EFC.Repositories;
using WebApplication1.Shared.Domain.Repositories;
using WebApplication1.Shared.Infrastructure.Interfaces.AspNetCore.Configuration;
using WebApplication1.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using WebApplication1.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using WebApplication1.Shared.Resources.Errors;
using WebApplication1.Shared.Resources;
using ProblemDetailsFactory = WebApplication1.Shared.Interfaces.Rest.ProblemDetails.ProblemDetailsFactory;

var builder = WebApplication.CreateBuilder(args);

// ---- Routing & Controllers ----
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services
    .AddControllers(options => options.Conventions.Add(new KebabCaseRouteNamingConvention()))
    .AddDataAnnotationsLocalization();

builder.Services.AddProblemDetails();

// ---- Database ----
builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(connectionString))
        throw new InvalidOperationException("Database connection string is not configured.");

    options.UseMySQL(connectionString)
        .UseLoggerFactory(serviceProvider.GetRequiredService<ILoggerFactory>())
        .EnableDetailedErrors();

    if (builder.Environment.IsDevelopment())
        options.EnableSensitiveDataLogging();
});

// ---- Localization ----
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddSingleton<IStringLocalizer<ErrorMessages>, StringLocalizer<ErrorMessages>>();
builder.Services.AddSingleton<IStringLocalizer<CommonMessages>, StringLocalizer<CommonMessages>>();

// ---- ProblemDetailsFactory ----
builder.Services.AddSingleton<ProblemDetailsFactory>();

// ---- Swagger ----
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "WebApplication1",
        Version = "v1",
        Description = "Hertz Rental Orders API"
    });
    options.EnableAnnotations();
});

// ---- Dependency Injection ----
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IRentalOrderRepository, RentalOrderRepository>();
builder.Services.AddScoped<IRentalOrderCommandService, RentalOrderCommandService>();

var app = builder.Build();

// ---- Migrations ----
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
}

// ---- Localization ----
var supportedCultures = new[] { "en", "es" };
app.UseRequestLocalization(new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures));

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();