using WebApplication1.Services.Domain.Model.Commands;
using WebApplication1.Services.Domain.Model.ValueObjects;
using WebApplication1.Shared.Domain.Model.Entities;

namespace WebApplication1.Services.Domain.Model.Aggregates;

/// <summary>
///     Aggregate root representing a vehicle rental order in the Hertz system.
/// </summary>
/// <remarks>Sebastian Pinedo</remarks>
public class RentalOrder : IAuditableEntity
{
    // ---- Properties ----

    /// <summary>Gets the rental order unique identifier.</summary>
    public int Id { get; private set; }

    /// <summary>Gets the customer name.</summary>
    public string Customer { get; private set; }

    /// <summary>Gets the vehicle type.</summary>
    public EVehicles VehiclesId { get; private set; }

    /// <summary>Gets the vehicle plate.</summary>
    public string Plate { get; private set; }

    /// <summary>Gets the requested date.</summary>
    public DateTimeOffset RequestedAt { get; private set; }

    /// <summary>Gets the delivery address.</summary>
    public RentalOrderAddress Address { get; private set; }

    /// <summary>Gets the rental amount.</summary>
    public double Amount { get; private set; }

    // ---- IAuditableEntity ----
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    // ---- EF Core constructor ----
    protected RentalOrder()
    {
        Customer = string.Empty;
        Plate = string.Empty;
        Address = new RentalOrderAddress();
    }

    /// <summary>
    ///     Creates a new rental order from a command.
    /// </summary>
    /// <param name="command">The command containing rental order data.</param>
    public RentalOrder(CreateRentalOrderCommand command)
    {
        Customer = command.Customer;
        VehiclesId = command.VehiclesId;
        Plate = command.Plate;
        RequestedAt = command.RequestedAt;
        Address = new RentalOrderAddress(command.Street, command.City, command.PostalCode);
        Amount = command.Amount;
    }
}
