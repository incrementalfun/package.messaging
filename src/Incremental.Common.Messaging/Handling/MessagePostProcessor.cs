using System.Threading;
using System.Threading.Tasks;
using Incremental.Common.Messaging.Client;
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
        private readonly IMessagingClientFactory _messagingClientFactory;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="messagingClientFactory"></param>
        public MessagePostProcessor(IMessagingClientFactory messagingClientFactory)
        {
            _messagingClientFactory = messagingClientFactory;
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
            var sender = await _messagingClientFactory.GetSender(message.Receipt.Queue, cancellationToken);
            
            await sender.MarkAsDelivered(message.Receipt.Id, cancellationToken);

            if (message.HasFollowingSteps)
            {
                foreach (var step in message.FollowingSteps())
                {
                    await sender.Send(message, Groups.Default, cancellationToken);
                }
            }
        }
    }
}