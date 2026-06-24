namespace WebApplication1.Services.Interfaces.REST.Resources;

/// <summary>
///     Input representation to register a new rental order.
/// </summary>
/// <remarks>Sebastian Pinedo</remarks>
public record CreateRentalOrderResource(
    string Customer,
    int VehiclesId,
    string Plate,
    DateTimeOffset RequestedAt,
    string Street,
    string City,
    string PostalCode,
    double Amount);
