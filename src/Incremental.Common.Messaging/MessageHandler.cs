using System.Threading.Tasks;
using MassTransit;

namespace Incremental.Common.Messaging
{
    /// <summary>
    ///     Message handler.
    /// </summary>
    /// <typeparam name="TMessage">Derived of <see cref="Message"/></typeparam>
    public abstract class MessageHandler<TMessage> : IConsumer<TMessage> where TMessage: Message
    {
        public abstract Task Consume(ConsumeContext<TMessage> context);
    }
}