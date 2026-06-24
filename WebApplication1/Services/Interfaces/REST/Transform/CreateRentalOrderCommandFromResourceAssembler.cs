using WebApplication1.Services.Domain.Model.Commands;
using WebApplication1.Services.Domain.Model.ValueObjects;
using WebApplication1.Services.Interfaces.REST.Resources;

namespace WebApplication1.Services.Interfaces.REST.Transform;

/// <summary>
///     Assembles a <see cref="CreateRentalOrderCommand"/> from a <see cref="CreateRentalOrderResource"/>.
/// </summary>
/// <remarks>Sebastian Pinedo</remarks>
public static class CreateRentalOrderCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts a <see cref="CreateRentalOrderResource"/> to a <see cref="CreateRentalOrderCommand"/>.
    /// </summary>
    /// <param name="resource">The input resource.</param>
    /// <returns>The command.</returns>
    public static CreateRentalOrderCommand ToCommandFromResource(CreateRentalOrderResource resource) =>
        new(resource.Customer,
            (EVehicles)resource.VehiclesId,
            resource.Plate,
            resource.RequestedAt,
            resource.Street,
            resource.City,
            resource.PostalCode,
            resource.Amount);
}
