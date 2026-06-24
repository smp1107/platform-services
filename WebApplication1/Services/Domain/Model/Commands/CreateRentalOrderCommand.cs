using WebApplication1.Services.Domain.Model.ValueObjects;

namespace WebApplication1.Services.Domain.Model.Commands;

public record CreateRentalOrderCommand(
    string Customer,
    EVehicles VehiclesId,
    string Plate,
    DateTimeOffset RequestedAt,
    string Street,
    string City,
    string PostalCode,
    double Amount);