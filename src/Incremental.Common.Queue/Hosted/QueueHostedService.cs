using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Incremental.Common.Queue.Hosted.Options;
using Incremental.Common.Sourcing.Events.Contract;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Incremental.Common.Queue.Hosted
{
    /// <summary>
    /// Queue service that retrieves events from the queue and launches them as external events.
    /// </summary>
    public class QueueHostedService : BackgroundService
    {
        private readonly ILogger<QueueHostedService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly QueueOptions _queueOptions;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="scopeFactory"></param>
        /// <param name="queueOptions"></param>
        public QueueHostedService(ILogger<QueueHostedService> logger, IServiceScopeFactory scopeFactory, IOptions<QueueOptions> queueOptions)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _queueOptions = queueOptions.Value;
        }

        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var serviceScope = _scopeFactory.CreateScope();

            var sqs = serviceScope.ServiceProvider.GetRequiredService<IAmazonSQS>();
            var eventBus = serviceScope.ServiceProvider.GetRequiredService<IEventBus>();
            
            while (!stoppingToken.IsCancellationRequested)
            {
                var response = await sqs.ReceiveMessageAsync(new ReceiveMessageRequest
                {
                    QueueUrl = Queues.Services,
                    MaxNumberOfMessages = 1
                }, stoppingToken);

                var message = Model.Message.FromSerialized(response.Messages.First().Body);

                if (_queueOptions.TypeDictionary.TryGetValue(message.EventType, out var type))
                {
                    var @event =  Convert.ChangeType(message.EventData, type);

                    try
                    {
                        await eventBus.Publish(@event as IExternalEvent);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Unhandled exception handling external event from queue. (@event)", @event);
                    }
                }
            }
        }
    }
}