using WebApplication1.Services.Domain.Model.Aggregates;
using WebApplication1.Services.Domain.Model.Commands;
using WebApplication1.Services.Domain.Model.Errors;
using WebApplication1.Shared.Application.Model;

namespace WebApplication1.Services.Application.CommandServices;

/// <summary>
///     Application service handling write operations for the Services bounded context.
/// </summary>
/// <remarks>Sebastian Pinedo</remarks>
public interface IRentalOrderCommandService
{
    /// <summary>
    ///     Handles the creation of a new rental order.
    /// </summary>
    /// <param name="command">The command containing rental order data.</param>
    /// <returns>A result containing the created rental order or an error.</returns>
    Task<Result<RentalOrder>> Handle(CreateRentalOrderCommand command);
}