using MediatR;

namespace Incremental.Common.Messaging.Messages
{
    /// <summary>
    ///     Message.
    /// </summary>
    public record Message : IRequest
    {
        /// <summary>
        ///     Receipt of the message in the queue.
        /// </summary>
        public (string Queue, string Id) Receipt { get; init; }
    }
}