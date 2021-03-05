using System;
using System.Threading;
using System.Threading.Tasks;

namespace Incremental.Common.Queues.Service.Contract
{
    /// <summary>
    ///     Queue receiver service.
    /// </summary>
    public interface IQueueReceiver
    {
        /// <summary>
        ///     Count of how many messages are in the queue right now.
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<int> Count(string queue, CancellationToken cancellationToken = default);

        public Task<TimeSpan> GetVisibilityTimeSpan(string queue, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Receives a specified quantity of messages from the queue.
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="quantity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<(string body, string type, (string queue, string id) receipt)> Receive(string queue, int quantity,
            CancellationToken cancellationToken = default);
    }
}