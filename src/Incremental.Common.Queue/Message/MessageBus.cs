using System.Threading;
using System.Threading.Tasks;
using Incremental.Common.Queue.Message.Contract;
using Incremental.Common.Queue.Service.Contract;

namespace Incremental.Common.Queue.Message
{
    internal class MessageBus : IMessageBus
    {
        private readonly IQueueSender _queueSender;

        public MessageBus(IQueueSender queueSender)
        {
            _queueSender = queueSender;
        }

        public async Task Send<TMessage>(string queue, TMessage message, CancellationToken cancellationToken = default) where TMessage : IMessage
        {
            await _queueSender.Send(queue, message, Groups.Default, cancellationToken);
        }

        public async Task Success((string queue, string id) receipt, CancellationToken cancellationToken = default)
        {
            await _queueSender.MarkAsDelivered(receipt.queue, receipt.id, cancellationToken);
        }
    }
}