using System.Threading;
using System.Threading.Tasks;

namespace Incremental.Common.Messaging
{
    /// <summary>
    /// Message bus.
    /// </summary>
    public interface IMessageBus
    {
        /// <summary>
        /// Send a message to the queues.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TMessage"></typeparam>
        /// <returns></returns>
        Task Send<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : Message;
    }
}