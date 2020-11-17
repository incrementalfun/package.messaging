using System.Threading;
using System.Threading.Tasks;

namespace Incremental.Common.Queue.Message.Contract
{
    /// <summary>
    /// Bus for sending message requests.
    /// </summary>
    public interface IMessageBus
    {
        /// <summary>
        /// Send a message to the queue.
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TMessage"></typeparam>
        /// <returns></returns>
        Task Send<TMessage>(string queue, TMessage message, CancellationToken cancellationToken = default) where TMessage : IMessage;

        /// <summary>
        /// When called signals to the queue that a message has been handled.
        /// </summary>
        /// <param name="receipt"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task Success((string queue, string id) receipt, CancellationToken cancellationToken = default);
    }
}