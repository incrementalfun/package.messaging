using System.Threading;

namespace Incremental.Common.Messaging.Client
{
    /// <summary>
    /// Factory of messaging clients.
    /// </summary>
    public interface IMessagingClientFactory
    {
        /// <summary>
        /// Gets a <see cref="IMessageSender"/> for the provided queue.
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public IMessageSender GetSender(string queue, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Gets a <see cref="IMessageReceiver"/> for the provided queue.
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public IMessageReceiver GetReceiver(string queue, CancellationToken cancellationToken = default);
    }
}