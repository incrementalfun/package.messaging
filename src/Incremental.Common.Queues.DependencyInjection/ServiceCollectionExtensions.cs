using System.Reflection;
using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.SQS;
using Incremental.Common.Queues.Messages;
using Incremental.Common.Queues.Messages.Contract;
using Incremental.Common.Queues.Service;
using Incremental.Common.Queues.Service.Contract;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Incremental.Common.Queues.DependencyInjection
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
        public static IServiceCollection AddQueues(this IServiceCollection services, IConfiguration configuration, params Assembly[] assemblies)
        {
            services.AddDefaultAWSOptions(new AWSOptions
            {
                Region = RegionEndpoint.EUWest1,
                Credentials = new BasicAWSCredentials(configuration["AWS_ACCESS_KEY"], configuration["AWS_SECRET_KEY"])
            });

            services.RegisterQueues(assemblies);

            return services;
        }

        /// <summary>
        ///     Configures all the related services necessary for queues to work. Requires credentials to be passed.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="accessKey"></param>
        /// <param name="secretKey"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static IServiceCollection AddQueues(this IServiceCollection services, string accessKey, string secretKey, params Assembly[] assemblies)
        {
            services.AddDefaultAWSOptions(new AWSOptions
            {
                Region = RegionEndpoint.EUWest1,
                Credentials = new BasicAWSCredentials(accessKey, secretKey)
            });

            services.RegisterQueues(assemblies);

            return services;
        }

        private static IServiceCollection RegisterQueues(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddMediatR(assemblies);

            services.AddScoped<IMessageBus, MessageBus>();

            services.AddAWSService<IAmazonSQS>();

            services.AddScoped<IQueueSender, QueueService>();
            services.AddScoped<IQueueReceiver, QueueService>();

            return services;
        }
    }
}