using System;
using System.Threading;
using System.Threading.Tasks;

namespace Incremental.Common.Messaging
{
    /// <summary>
    ///     Messaging receiver service.
    /// </summary>
    public interface IMessageReceiver
    {
        /// <summary>
        ///     Count of how many messages are in the queue right now.
        /// </summary>
        /// <param name="cancellationToken">
        ///     <see cref="CancellationToken" />
        /// </param>
        /// <returns></returns>
        public Task<int> Count(CancellationToken cancellationToken = default);

        /// <summary>
        ///     Returns the visibility timespan of the queue.
        /// </summary>
        /// <param name="cancellationToken">
        ///     <see cref="CancellationToken" />
        /// </param>
        /// <returns></returns>
        public Task<TimeSpan> GetVisibilityTimeSpan(CancellationToken cancellationToken = default);

        /// <summary>
        ///     Receives a specified quantity of messages from the queue.
        /// </summary>
        /// <param name="quantity">Quantity of messages to receive.</param>
        /// <param name="cancellationToken">
        ///     <see cref="CancellationToken" />
        /// </param>
        /// <returns></returns>
        public Task<(string body, string type, (string queue, string id) receipt)> Receive(int quantity,
            CancellationToken cancellationToken = default);
    }
}