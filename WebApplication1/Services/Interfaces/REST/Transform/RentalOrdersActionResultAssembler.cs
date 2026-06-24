using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services.Domain.Model.Aggregates;
using WebApplication1.Services.Domain.Model.Errors;
using WebApplication1.Shared.Application.Model;
using WebApplication1.Shared.Interfaces.Rest.ProblemDetails;

namespace WebApplication1.Services.Interfaces.REST.Transform;

/// <summary>
///     Converts <see cref="Result{RentalOrder}"/> instances to <see cref="IActionResult"/> responses.
/// </summary>
/// <remarks>Sebastian Pinedo</remarks>
public static class RentalOrdersActionResultAssembler
{
    public static IActionResult ToActionResult(
        Result<RentalOrder> result,
        ControllerBase controller,
        Func<RentalOrder, IActionResult> onSuccess,
        ProblemDetailsFactory problemDetailsFactory)
    {
        if (result.IsSuccess && result.Value is not null)
            return onSuccess(result.Value);

        var statusCode = (ServicesErrors)result.Error! switch
        {
            ServicesErrors.InvalidAmount => StatusCodes.Status400BadRequest,
            ServicesErrors.InvalidRequestedAt => StatusCodes.Status400BadRequest,
            ServicesErrors.DuplicateCustomerAndVehicle => StatusCodes.Status400BadRequest,
            ServicesErrors.PlateBelongsToDifferentVehicle => StatusCodes.Status400BadRequest,
            ServicesErrors.InvalidVehicleId => StatusCodes.Status400BadRequest,
            ServicesErrors.OperationCancelled => StatusCodes.Status400BadRequest,
            ServicesErrors.DatabaseError => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError
        };

        return problemDetailsFactory.CreateProblemDetails(
            controller,
            statusCode,
            result.Error!,
            result.Message);
    }
}