using Microsoft.Extensions.DependencyInjection;

namespace Incremental.Common.Messaging
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMessaging(this IServiceCollection services)
        {
            return services.AddTransient<IMessageBus, MessageBus>();
        }

    }
}