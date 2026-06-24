using Microsoft.EntityFrameworkCore;
using WebApplication1.Services.Domain.Model.Aggregates;
using WebApplication1.Services.Domain.Model.ValueObjects;
using WebApplication1.Services.Domain.Repositories;
using WebApplication1.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using WebApplication1.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace WebApplication1.Services.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     EF Core implementation of <see cref="IRentalOrderRepository"/>.
/// </summary>
/// <remarks>Sebastian Pinedo</remarks>
public class RentalOrderRepository(AppDbContext context)
    : BaseRepository<RentalOrder>(context), IRentalOrderRepository
{
    /// <inheritdoc/>
    public async Task<bool> ExistsByCustomerAndVehiclesIdAsync(string customer, EVehicles vehiclesId)
        => await Context.Set<RentalOrder>()
            .AnyAsync(r => r.Customer == customer && r.VehiclesId == vehiclesId);

    /// <inheritdoc/>
    public async Task<bool> ExistsByPlateAndDifferentVehicleAsync(string plate, EVehicles vehiclesId)
        => await Context.Set<RentalOrder>()
            .AnyAsync(r => r.Plate == plate && r.VehiclesId != vehiclesId);
}