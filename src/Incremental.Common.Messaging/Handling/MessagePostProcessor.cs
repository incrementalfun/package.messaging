using System.Threading;
using System.Threading.Tasks;
using Incremental.Common.Messaging.Client;
using Incremental.Common.Messaging.Messages;
using MediatR;
using MediatR.Pipeline;

namespace Incremental.Common.Messaging.Handling
{
    /// <summary>
    /// Handles the post processing of a message. Necessary to mark a message as success.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public abstract class MessagePostProcessor<TMessage> : IRequestPostProcessor<TMessage, Unit> where TMessage : Message
    {
        private readonly IMessageSender _messageSender;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="messageSender"><see cref="IMessageSender"/></param>
        public MessagePostProcessor(IMessageSender messageSender)
        {
            _messageSender = messageSender;
        }

        /// <inheritdoc />
        public abstract Task Process(TMessage message, Unit response, CancellationToken cancellationToken);

        /// <summary>
        /// If called, it will mark the message as delivered.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task Success(TMessage message, CancellationToken cancellationToken = default)
        {
            await _messageSender.MarkAsDelivered(message.Receipt.Queue, message.Receipt.Id, cancellationToken);

            if (message.HasFollowingSteps)
            {
                foreach (var step in message.FollowingSteps())
                {
                    await _messageSender.Send(message.Receipt.Queue, message, "default", cancellationToken);
                }
            }
        }
    }
}