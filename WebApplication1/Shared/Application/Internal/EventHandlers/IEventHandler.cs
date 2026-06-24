using Cortex.Mediator.Notifications;
using WebApplication1.Shared.Domain.Model.Events;

namespace WebApplication1.Shared.Application.Internal.EventHandlers;

public interface IEventHandler<in TEvent> : INotificationHandler<TEvent> where TEvent : IEvent
{
}