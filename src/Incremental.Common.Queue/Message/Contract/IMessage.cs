using MediatR;

namespace Incremental.Common.Queue.Message.Contract
{
    /// <summary>
    /// Message.
    /// </summary>
    public interface IMessage : IRequest
    {
        /// <summary>
        /// Receipt of the message in the queue.
        /// </summary>
        public (string queue, string id) Receipt { get; set; }
    }
}