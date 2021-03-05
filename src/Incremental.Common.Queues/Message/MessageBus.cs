using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Incremental.Common.Queues.Message.Contract;
using Incremental.Common.Queues.Service.Contract;

namespace Incremental.Common.Queues.Message
{
    public class MessageBus : IMessageBus
    {
        private readonly IQueueSender _queueSender;

        public MessageBus(IQueueSender queueSender)
        {
            _queueSender = queueSender;
        }

        public async Task Send<TMessage>(string queue, TMessage message, CancellationToken cancellationToken = default) where TMessage : Contract.Message
        {
            await _queueSender.Send(queue, message, Groups.Default, cancellationToken);
        }
        
        public async Task Send<TMessage>(string queue, IEnumerable<TMessage> messages, CancellationToken cancellationToken = default) where TMessage : Contract.Message
        {
            foreach (var message in messages)
            {
                await _queueSender.Send(queue, message, Groups.Default, cancellationToken);
            }
        }

        public async Task Success((string queue, string id) receipt, CancellationToken cancellationToken = default)
        {
            await _queueSender.MarkAsDelivered(receipt.queue, receipt.id, cancellationToken);
        }
    }
}