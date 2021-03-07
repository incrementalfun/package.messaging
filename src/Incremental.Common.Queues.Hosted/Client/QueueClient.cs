using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Incremental.Common.Queues.Client;
using Microsoft.Extensions.Logging;
using Message = Incremental.Common.Queues.Messages.Message;

namespace Incremental.Common.Queues.Hosted.Client
{
    internal class QueueClient : IQueueSender, IQueueReceiver
    {
        private readonly ILogger<QueueClient> _logger;
        private readonly IAmazonSQS _sqs;

        public QueueClient(ILogger<QueueClient> logger, IAmazonSQS sqs)
        {
            _logger = logger;
            _sqs = sqs;
        }

        public async Task<int> Count(string queue, CancellationToken cancellationToken = default)
        {
            var attributes = await _sqs.GetQueueAttributesAsync(queue,
                new List<string> {QueueAttributeName.ApproximateNumberOfMessages},
                cancellationToken);

            return attributes.ApproximateNumberOfMessages;
        }

        public async Task<TimeSpan> GetVisibilityTimeSpan(string queue, CancellationToken cancellationToken = default)
        {
            var desiredAttributes = new List<string> {QueueAttributeName.VisibilityTimeout};

            var queueAttributes = await _sqs.GetQueueAttributesAsync(queue, desiredAttributes, cancellationToken);

            return TimeSpan.FromSeconds(queueAttributes.VisibilityTimeout);
        }

        public async Task<(string body, string type, (string queue, string id) receipt)> Receive(string queue, int quantity,
            CancellationToken cancellationToken = default)
        {
            var response = await _sqs.ReceiveMessageAsync(new ReceiveMessageRequest
            {
                QueueUrl = queue,
                MaxNumberOfMessages = 1,
                MessageAttributeNames = new List<string> {nameof(Type)}
            }, cancellationToken);

            if (!response.Messages.Any()) return default;

            var message = response.Messages.First();

            if (message.MessageAttributes.TryGetValue(nameof(Type), out var typeAttribute) && !string.IsNullOrWhiteSpace(typeAttribute.StringValue))
                return (message.Body, typeAttribute.StringValue, (queue, message.ReceiptHandle));

            return default;
        }

        public async Task Send(string queue, Message message, string groupId, CancellationToken cancellationToken = default)
        {
            var type = message.GetType().FullName;
            var body = JsonSerializer.Serialize(message as object);

            await Send(queue, body, type, groupId, cancellationToken);
        }

        public async Task MarkAsDelivered(string queue, string receiptHandle, CancellationToken cancellationToken = default)
        {
            await _sqs.DeleteMessageAsync(queue, receiptHandle, cancellationToken);
        }

        private async Task Send(string queue, string body, string type, string groupId, CancellationToken cancellationToken)
        {
            var response = await _sqs.SendMessageAsync(new SendMessageRequest(queue, body)
            {
                MessageGroupId = groupId,
                MessageDeduplicationId = Guid.NewGuid().ToString(),
                MessageAttributes = new Dictionary<string, MessageAttributeValue>
                {
                    {
                        nameof(Type), new MessageAttributeValue
                        {
                            StringValue = type,
                            DataType = "String"
                        }
                    }
                }
            }, cancellationToken);

            if (string.IsNullOrWhiteSpace(response.MessageId)) _logger.LogError("Message has not been delivered to the queue ({@message})", body);
        }
    }
}