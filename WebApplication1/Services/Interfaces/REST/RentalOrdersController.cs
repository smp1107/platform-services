using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services.Application.CommandServices;
using WebApplication1.Services.Interfaces.REST.Resources;
using WebApplication1.Services.Interfaces.REST.Transform;
using WebApplication1.Shared.Interfaces.Rest.ProblemDetails;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApplication1.Services.Interfaces.REST;

/// <summary>
///     REST controller exposing the rental orders endpoint.
/// </summary>
/// <remarks>Sebastian Pinedo</remarks>
[ApiController]
[Route("api/v1/rental-orders")]
[Produces("application/json")]
[SwaggerTag("Available Rental Orders endpoints")]
public class RentalOrdersController(
    IRentalOrderCommandService rentalOrderCommandService,
    ProblemDetailsFactory problemDetailsFactory) : ControllerBase
{
    /// <summary>
    ///     Creates a new rental order.
    /// </summary>
    /// <param name="resource">The rental order data.</param>
    /// <returns>The created rental order or an error response.</returns>
    [HttpPost]
    [SwaggerOperation(Summary = "Create a rental order", OperationId = "CreateRentalOrder")]
    [SwaggerResponse(201, "Rental order created successfully", typeof(RentalOrderResource))]
    [SwaggerResponse(400, "Invalid rental order data")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<IActionResult> CreateRentalOrder([FromBody] CreateRentalOrderResource resource)
    {
        var command = CreateRentalOrderCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await rentalOrderCommandService.Handle(command);

        return RentalOrdersActionResultAssembler.ToActionResult(
            result,
            this,
            rentalOrder => CreatedAtAction(
                nameof(CreateRentalOrder),
                new { id = rentalOrder.Id },
                RentalOrderResourceFromEntityAssembler.ToResourceFromEntity(rentalOrder)),
            problemDetailsFactory);
    }
}