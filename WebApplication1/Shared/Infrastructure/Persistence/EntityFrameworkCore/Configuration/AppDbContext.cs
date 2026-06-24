using Microsoft.EntityFrameworkCore;
using WebApplication1.Services.Infrastructure.Persistence.EFC.Configuration.Extensions;
using WebApplication1.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using WebApplication1.Shared.Infrastructure.Persistence.EntityFrameworkCore.Interceptors;

namespace WebApplication1.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

/// <summary>
///     Application database context
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.AddInterceptors(new AuditableEntityInterceptor());
        base.OnConfiguring(builder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyServicesConfiguration();

        builder.UseSnakeCaseNamingConvention();
    }
}