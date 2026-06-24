namespace WebApplication1.Services.Domain.Model.Errors;

public enum ServicesErrors
{
    None,
    DuplicateCustomerAndVehicle,
    InvalidAmount,
    InvalidRequestedAt,
    PlateBelongsToDifferentVehicle,
    InvalidVehicleId,
    OperationCancelled,
    DatabaseError,
    InternalServerError
}