using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;

namespace Incremental.Common.Messaging.Client
{
    internal class MessagingClient : IMessageSender, IMessageReceiver
    {
        private readonly ILogger<MessagingClient> _logger;
        private readonly string _queue;
        private readonly IAmazonSQS _sqs;

        public MessagingClient(ILogger<MessagingClient> logger, IAmazonSQS sqs, string queue)
        {
            _logger = logger;
            _sqs = sqs;
            _queue = queue;
        }

        public async Task<int> Count(CancellationToken cancellationToken = default)
        {
            var attributes = await _sqs.GetQueueAttributesAsync(_queue,
                new List<string> {QueueAttributeName.ApproximateNumberOfMessages},
                cancellationToken);

            return attributes.ApproximateNumberOfMessages;
        }

        public async Task<TimeSpan> GetVisibilityTimeSpan(CancellationToken cancellationToken = default)
        {
            var desiredAttributes = new List<string> {QueueAttributeName.VisibilityTimeout};

            var queueAttributes = await _sqs.GetQueueAttributesAsync(_queue, desiredAttributes, cancellationToken);

            return TimeSpan.FromSeconds(queueAttributes.VisibilityTimeout);
        }

        public async Task<(string body, string type, (string queue, string id) receipt)> Receive(int quantity,
            CancellationToken cancellationToken = default)
        {
            var response = await _sqs.ReceiveMessageAsync(new ReceiveMessageRequest
            {
                QueueUrl = _queue,
                MaxNumberOfMessages = 1,
                MessageAttributeNames = new List<string> {nameof(Type)}
            }, cancellationToken);

            if (!response.Messages.Any()) return default;

            var message = response.Messages.First();

            if (message.MessageAttributes.TryGetValue(nameof(Type), out var typeAttribute) && !string.IsNullOrWhiteSpace(typeAttribute.StringValue))
                return (message.Body, typeAttribute.StringValue, (_queue, message.ReceiptHandle));

            return default;
        }

        public async Task Send(Message message, string groupId, CancellationToken cancellationToken = default)
        {
            var type = message.GetType().FullName;
            var body = JsonSerializer.Serialize(message as object);

            await Send(body, type, groupId, cancellationToken);
        }

        public async Task MarkAsDelivered(string receiptHandle, CancellationToken cancellationToken = default)
        {
            await _sqs.DeleteMessageAsync(_queue, receiptHandle, cancellationToken);
        }

        private async Task Send(string body, string type, string groupId, CancellationToken cancellationToken)
        {
            var response = await _sqs.SendMessageAsync(new SendMessageRequest(_queue, body)
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