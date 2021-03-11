using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Incremental.Common.Messaging.Handling
{
    /// <summary>
    ///     Message handler.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public abstract class MessageHandler<TMessage> : IRequestHandler<TMessage> where TMessage : Message
    {
        public abstract Task<Unit> Handle(TMessage message, CancellationToken cancellationToken);
    }
}