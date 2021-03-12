using System.Threading;
using System.Threading.Tasks;

namespace Incremental.Common.Messaging
{
    /// <summary>
    ///     Messaging sender service.
    /// </summary>
    public interface IMessageSender
    {
        /// <summary>
        ///     Sends an event to the queues.
        /// </summary>
        /// <param name="message"><see cref="Message" /> to send.</param>
        /// <param name="groupId"></param>
        /// <param name="cancellationToken">
        ///     <see cref="CancellationToken" />
        /// </param>
        /// <returns></returns>
        public Task Send(Message message, string groupId, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Marks a retrieved message as delivered.
        /// </summary>
        /// <param name="receiptHandle">Receipt identifier used to mark a message as delivered.</param>
        /// <param name="cancellationToken">
        ///     <see cref="CancellationToken" />
        /// </param>
        /// <returns></returns>
        public Task MarkAsDelivered(string receiptHandle, CancellationToken cancellationToken = default);
    }
}