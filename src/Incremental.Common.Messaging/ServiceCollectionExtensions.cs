using Microsoft.Extensions.DependencyInjection;

namespace Incremental.Common.Messaging
{
    /// <summary>
    ///     Service collection extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Add messaging utilities to services.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMessaging(this IServiceCollection services)
        {
            services.AddTransient<IMessageBus, MessageBus>();
            services.AddScoped(typeof(IRequestBus<,>), typeof(RequestBus<,>));

            return services;
        }
    }
}