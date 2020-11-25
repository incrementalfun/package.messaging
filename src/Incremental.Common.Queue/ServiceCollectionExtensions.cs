using System.Linq;
using System.Reflection;
using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.SQS;
using Incremental.Common.Queue.Channel;
using Incremental.Common.Queue.Channel.Contract;
using Incremental.Common.Queue.Message;
using Incremental.Common.Queue.Message.Contract;
using Incremental.Common.Queue.Service;
using Incremental.Common.Queue.Service.Contract;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Incremental.Common.Queue
{
    /// <summary>
    /// Registers queue management.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configures all the related services necessary for queues to work. Credentials are sourced from configuration automatically.
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
        /// Configures all the related services necessary for queues to work. Requires credentials to be passed.
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

        /// <summary>
        /// Registers a channel queue.
        /// </summary>
        /// <param name="services"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IServiceCollection AddChannelQueue<T>(this IServiceCollection services)
        {
            services.AddSingleton<ChannelQueue<T>>();

            services.AddScoped<IQueueReader<T>>(serviceProvider => serviceProvider.GetRequiredService<ChannelQueue<T>>());
            services.AddScoped<IQueueWriter<T>>(serviceProvider => serviceProvider.GetRequiredService<ChannelQueue<T>>());

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