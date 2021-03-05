using MediatR;

namespace Incremental.Common.Queues.Messages
{
    /// <summary>
    ///     Message.
    /// </summary>
    public record Message : IRequest
    {
        /// <summary>
        ///     Receipt of the message in the queue.
        /// </summary>
        public (string queue, string id) Receipt { get; init; }
    }
}