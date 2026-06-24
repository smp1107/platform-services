using Microsoft.EntityFrameworkCore;
using WebApplication1.Services.Domain.Model.Aggregates;
using WebApplication1.Services.Domain.Model.ValueObjects;

namespace WebApplication1.Services.Infrastructure.Persistence.EFC.Configuration.Extensions;

/// <summary>
///     EF Core model configuration for the Services bounded context.
/// </summary>
/// <remarks>Sebastian Pinedo</remarks>
public static class ModelBuilderExtensions
{
    /// <summary>
    ///     Applies the Services bounded context entity configurations.
    /// </summary>
    /// <param name="builder">The model builder instance.</param>
    public static void ApplyServicesConfiguration(this ModelBuilder builder)
    {
        builder.Entity<RentalOrder>(entity =>
        {
            entity.ToTable("rental_orders");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Id)
                .HasColumnName("id")
                .IsRequired()
                .ValueGeneratedOnAdd();
            entity.Property(r => r.Customer)
                .HasColumnName("customer")
                .IsRequired()
                .HasMaxLength(90);
            entity.Property(r => r.VehiclesId)
                .HasColumnName("vehicles_id")
                .IsRequired()
                .HasConversion<int>();
            entity.Property(r => r.Plate)
                .HasColumnName("plate")
                .HasMaxLength(12);
            entity.Property(r => r.RequestedAt)
                .HasColumnName("requested_at")
                .IsRequired();
            entity.OwnsOne(r => r.Address,
                address =>
                {
                    address.WithOwner().HasForeignKey("Id");

                    address.Property(a => a.Street)
                        .HasColumnName("address_street")
                        .IsRequired()
                        .HasMaxLength(40);

                    address.Property(a => a.City)
                        .HasColumnName("address_city")
                        .IsRequired()
                        .HasMaxLength(40);

                    address.Property(a => a.PostalCode)
                        .HasColumnName("address_postal_code")
                        .IsRequired()
                        .HasMaxLength(40);
                });
            entity.Property(r => r.Amount)
                .HasColumnName("amount")
                .IsRequired();
            entity.Property(r => r.CreatedAt)
                .HasColumnName("created_at");
            entity.Property(r => r.UpdatedAt)
                .HasColumnName("updated_at");
        });
    }
}
