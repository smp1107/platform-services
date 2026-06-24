using WebApplication1.Services.Application.CommandServices;
using WebApplication1.Services.Domain.Model.Aggregates;
using WebApplication1.Services.Domain.Model.Commands;
using WebApplication1.Services.Domain.Model.Errors;
using WebApplication1.Services.Domain.Repositories;
using WebApplication1.Shared.Application.Model;
using WebApplication1.Shared.Domain.Repositories;

namespace WebApplication1.Services.Application.Internal.CommandServices;

/// <summary>
///     Default implementation of <see cref="IRentalOrderCommandService"/>.
/// </summary>
/// <remarks>Sebastian Pinedo</remarks>
public class RentalOrderCommandService(
    IRentalOrderRepository rentalOrderRepository,
    IUnitOfWork unitOfWork) : IRentalOrderCommandService
{
    /// <inheritdoc/>
    public async Task<Result<RentalOrder>> Handle(CreateRentalOrderCommand command)
    {
        // Rule 1: Amount must be greater than zero
        if (command.Amount <= 0)
            return Result<RentalOrder>.Failure(
                ServicesErrors.InvalidAmount,
                "Amount must be greater than zero.");

        // Rule 2: RequestedAt cannot be earlier than today
        if (command.RequestedAt.Date < DateTimeOffset.UtcNow.Date)
            return Result<RentalOrder>.Failure(
                ServicesErrors.InvalidRequestedAt,
                "Requested date cannot be earlier than today.");

        // Rule 3: No duplicate customer and vehicleId
        var duplicateCustomerVehicle = await rentalOrderRepository
            .ExistsByCustomerAndVehiclesIdAsync(command.Customer, command.VehiclesId);
        if (duplicateCustomerVehicle)
            return Result<RentalOrder>.Failure(
                ServicesErrors.DuplicateCustomerAndVehicle,
                "A rental order with the same customer and vehicle already exists.");

        // Rule 4: Plate cannot belong to a different vehicle
        var plateBelongsToDifferentVehicle = await rentalOrderRepository
            .ExistsByPlateAndDifferentVehicleAsync(command.Plate, command.VehiclesId);
        if (plateBelongsToDifferentVehicle)
            return Result<RentalOrder>.Failure(
                ServicesErrors.PlateBelongsToDifferentVehicle,
                "The plate already belongs to a different vehicle.");

        try
        {
            var rentalOrder = new RentalOrder(command);
            await rentalOrderRepository.AddAsync(rentalOrder);
            await unitOfWork.CompleteAsync();
            return Result<RentalOrder>.Success(rentalOrder);
        }
        catch (Exception ex)
        {
            return Result<RentalOrder>.Failure(ServicesErrors.InvalidAmount, "Amount must be greater than zero.");
        }
    }
}
