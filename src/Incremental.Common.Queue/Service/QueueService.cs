using System;
using System.Collections.Generic;
using System.Linq;
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
    /// <summary>
    /// Default implementation of IQueueSender and IQueueReceiver.
    /// </summary>
    public class QueueService : IQueueSender, IQueueReceiver
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
            var message = Message.FromExternalEvent(@event);

            await Send(message.Body, message.MessageType, queue, groupId, cancellationToken);
        }

        private async Task Send(string message, string type, string queue, string groupId, CancellationToken cancellationToken)
        {
            var response = await _sqs.SendMessageAsync(new SendMessageRequest(queue, message)
            {
                MessageGroupId = groupId,
                MessageDeduplicationId = Guid.NewGuid().ToString(),
                MessageAttributes = new Dictionary<string, MessageAttributeValue>
                {
                    {
                        nameof(Message.MessageType), new MessageAttributeValue
                        {
                            StringValue = type,
                            DataType = "String"
                        }
                    }
                }
            }, cancellationToken);

            if (string.IsNullOrWhiteSpace(response.MessageId))
            {
                _logger.LogWarning("Event has not been delivered to the queue (@message)", message);
            }
        }

        /// <inheritdoc />
        public async Task<Message> Receive(string queue, int quantity, CancellationToken cancellationToken = default)
        {
            var response = await _sqs.ReceiveMessageAsync(new ReceiveMessageRequest
            {
                QueueUrl = Queues.Services,
                MaxNumberOfMessages = 1,
                MessageAttributeNames = new List<string> {nameof(Message.MessageType)}
            }, cancellationToken);

            if (!response.Messages.Any())
            {
                return new Message();
            }

            var messageRaw = response.Messages.First();

            if (messageRaw.MessageAttributes.TryGetValue(nameof(Message.MessageType), out var messageAttributeValue))
            {
                var message = new Message
                {
                    MessageId = messageRaw.MessageId,
                    Body = messageRaw.Body,
                    MessageType = messageAttributeValue.StringValue,
                    ReceiptHandle = messageRaw.ReceiptHandle
                };

                return message;
            }

            return new Message();
        }

        /// <inheritdoc />
        public async Task MarkAsDelivered(string queue, string receiptHandle, CancellationToken cancellationToken = default)
        {
            await _sqs.DeleteMessageAsync(Queues.Services, receiptHandle, cancellationToken);
        }
    }
}