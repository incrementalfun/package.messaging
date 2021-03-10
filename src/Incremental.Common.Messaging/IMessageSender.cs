using System.Threading;
using System.Threading.Tasks;
using Incremental.Common.Messaging.Messages;

namespace Incremental.Common.Messaging.Client
{
    /// <summary>
    ///     Queue sender service.
    /// </summary>
    public interface IMessageSender
    {
        /// <summary>
        ///     Sends an event to the queues.
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="message"></param>
        /// <param name="groupId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task Send(string queue, Message message, string groupId, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Marks a retrieved message as delivered.
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="receiptHandle"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task MarkAsDelivered(string queue, string receiptHandle, CancellationToken cancellationToken = default);
    }
}