using MassTransit;

namespace Incremental.Common.Messaging
{
    /// <summary>
    /// External Event handler.
    /// </summary>
    /// <typeparam name="TExternalEvent"></typeparam>
    public interface IExternalEventHandler<in TExternalEvent> : IConsumer<TExternalEvent> where TExternalEvent : ExternalEvent
    {
    }
}