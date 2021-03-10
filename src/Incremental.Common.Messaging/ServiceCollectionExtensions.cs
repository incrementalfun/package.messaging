using System.Reflection;
using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.SQS;
using Incremental.Common.Messaging.Client;
using Incremental.Common.Messaging.Messages;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Incremental.Common.Messaging
{
    /// <summary>
    ///     Registers queue management.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Configures all the related services necessary for queues to work. Credentials are sourced from configuration automatically.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration, params Assembly[] assemblies)
        {
            services.AddDefaultAWSOptions(new AWSOptions
            {
                Region = RegionEndpoint.EUWest1,
                Credentials = new BasicAWSCredentials(configuration["AWS_ACCESS_KEY"], configuration["AWS_SECRET_KEY"])
            });

            services.RegisterQueues(assemblies);

            return services;
        }

        private static IServiceCollection RegisterQueues(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddAWSService<IAmazonSQS>();
            
            services.AddScoped<IMessageSender, MessagingQueueClient>();
            services.AddScoped<IMessageReceiver, MessagingQueueClient>();

            return services;
        }
    }
}