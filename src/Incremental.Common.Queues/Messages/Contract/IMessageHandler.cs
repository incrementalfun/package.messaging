using MediatR;

namespace Incremental.Common.Queues.Messages.Contract
{
    /// <summary>
    ///     Message handler.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public interface IMessageHandler<in TMessage> : IRequestHandler<TMessage> where TMessage : Message
    {
    }
}