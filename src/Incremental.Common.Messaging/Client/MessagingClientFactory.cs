using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Incremental.Common.Messaging.Client
{
    internal class MessagingClientFactory : IMessagingClientFactory
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public MessagingClientFactory(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task<IMessageSender> GetSender(string queue, CancellationToken cancellationToken = default)
        {
            return await GetMessagingClient(queue, cancellationToken);
        }

        public async Task<IMessageReceiver> GetReceiver(string queue, CancellationToken cancellationToken = default)
        {
            return await GetMessagingClient(queue, cancellationToken);
        }

        private async Task<MessagingClient> GetMessagingClient(string queue, CancellationToken cancellationToken = default)
        {
            using var scope = _scopeFactory.CreateScope();

            var logger = scope.ServiceProvider.GetRequiredService<ILogger<MessagingClient>>();
            var sqs = scope.ServiceProvider.GetRequiredService<IAmazonSQS>();

            var response = await sqs.GetQueueUrlAsync(queue, cancellationToken);

            return new MessagingClient(logger, sqs, response.QueueUrl);
        }
    }
}