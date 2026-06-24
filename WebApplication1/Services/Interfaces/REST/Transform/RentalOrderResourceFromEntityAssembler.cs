using WebApplication1.Services.Domain.Model.Aggregates;
using WebApplication1.Services.Interfaces.REST.Resources;

namespace WebApplication1.Services.Interfaces.REST.Transform;

/// <summary>
///     Assembles a <see cref="RentalOrderResource"/> from a <see cref="RentalOrder"/> entity.
/// </summary>
/// <remarks>Sebastian Pinedo</remarks>
public static class RentalOrderResourceFromEntityAssembler
{
    /// <summary>
    ///     Converts a <see cref="RentalOrder"/> entity to a <see cref="RentalOrderResource"/>.
    /// </summary>
    /// <param name="entity">The rental order entity.</param>
    /// <returns>The rental order resource.</returns>
    public static RentalOrderResource ToResourceFromEntity(RentalOrder entity) =>
        new(entity.Id,
            entity.Customer,
            (int)entity.VehiclesId,
            entity.Plate,
            entity.RequestedAt,
            entity.Address.Street,
            entity.Address.City,
            entity.Address.PostalCode);
}
