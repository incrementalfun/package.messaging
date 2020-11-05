using System.Threading;
using System.Threading.Tasks;
using Incremental.Common.Queue.Model;
using Incremental.Common.Sourcing.Events.Contract;

namespace Incremental.Common.Queue.Service.Contract
{
    /// <summary>
    /// Queue receiver service.
    /// </summary>
    public interface IQueueReceiver
    {
        /// <summary>
        /// Receives a specified quantity of messages from the queue.
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="quantity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<Message> Receive(string queue, int quantity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Marks a retrieved message as delivered.
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="receiptHandle"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task MarkAsDelivered(string queue, string receiptHandle, CancellationToken cancellationToken = default);
    }
}