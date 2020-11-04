using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Incremental.Common.Queue.Service.Contract;
using Incremental.Common.Sourcing.Events.Contract;
using Microsoft.Extensions.Logging;
using Message = Incremental.Common.Queue.Model.Message;

namespace Incremental.Common.Queue.Service
{
    /// <inheritdoc />
    public class QueueService : IQueueService
    {
        private readonly ILogger<QueueService> _logger;
        private readonly IAmazonSQS _sqs;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="sqs"></param>
        public QueueService(ILogger<QueueService> logger, IAmazonSQS sqs)
        {
            _logger = logger;
            _sqs = sqs;
        }

        /// <inheritdoc />
        public async Task Send(string queue, IExternalEvent @event, string groupId, CancellationToken cancellationToken = default)
        {
            var message = JsonSerializer.Serialize(Message.FromExternalEvent(@event));

            await Send(message, queue, groupId, cancellationToken);
        }

        private async Task Send(string message, string queue, string groupId, CancellationToken cancellationToken)
        {
            var response = await _sqs.SendMessageAsync(new SendMessageRequest(queue, message)
            {
                MessageGroupId = groupId
            }, cancellationToken);

            if (string.IsNullOrWhiteSpace(response.MessageId))
            {
                _logger.LogWarning("Event has not been delivered to the queue (@message)", message);
            }
        }
    }
}