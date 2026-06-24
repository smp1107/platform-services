namespace WebApplication1.Services.Interfaces.REST.Resources;

/// <summary>
///     Output representation of a <see cref="RentalOrder"/> exposed through the REST API.
///     Note: amount and audit fields are excluded per business requirements.
/// </summary>
/// <remarks>Sebastian Pinedo</remarks>
public record RentalOrderResource(
    int RentalOrderId,
    string Customer,
    int VehiclesId,
    string Plate,
    DateTimeOffset RequestedAt,
    string Street,
    string City,
    string PostalCode);
