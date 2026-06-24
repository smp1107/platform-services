using WebApplication1.Services.Domain.Model.Aggregates;
using WebApplication1.Services.Domain.Model.ValueObjects;
using WebApplication1.Shared.Domain.Repositories;

namespace WebApplication1.Services.Domain.Repositories;

/// <summary>
///     Persistence contract for the <see cref="RentalOrder"/> aggregate.
/// </summary>
/// <remarks>Sebastian Pinedo</remarks>
public interface IRentalOrderRepository : IBaseRepository<RentalOrder>
{
    /// <summary>
    ///     Checks if a rental order exists with the same customer and vehicle.
    /// </summary>
    Task<bool> ExistsByCustomerAndVehiclesIdAsync(string customer, EVehicles vehiclesId);

    /// <summary>
    ///     Checks if a plate is already assigned to a different vehicle.
    /// </summary>
    Task<bool> ExistsByPlateAndDifferentVehicleAsync(string plate, EVehicles vehiclesId);
}