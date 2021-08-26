using MassTransit;

namespace Incremental.Common.Messaging
{
    /// <summary>
    ///     Message handler.
    /// </summary>
    /// <typeparam name="TMessage">Derived of <see cref="Message" /></typeparam>
    public interface IMessageHandler<in TMessage> : IConsumer<TMessage> where TMessage : Message
    {
    }
}