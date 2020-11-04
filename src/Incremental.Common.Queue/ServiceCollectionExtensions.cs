using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.SQS;
using Incremental.Common.Queue.Service;
using Incremental.Common.Queue.Service.Contract;
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
        /// <returns></returns>
        public static IServiceCollection AddQueues(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDefaultAWSOptions(new AWSOptions
            {
                Credentials = new BasicAWSCredentials(configuration["AWS_ACCESS_KEY"], configuration["AWS_SECRET_KEY"])
            });

            services.RegisterQueues();
            
            return services;
        }

        /// <summary>
        /// Configures all the related services necessary for queues to work. Requires credentials to be passed.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="accessKey"></param>
        /// <param name="secretKey"></param>
        /// <returns></returns>
        public static IServiceCollection AddQueues(this IServiceCollection services, string accessKey, string secretKey)
        {
            services.AddDefaultAWSOptions(new AWSOptions
            {
                Credentials = new BasicAWSCredentials(accessKey, secretKey)
            });

            services.RegisterQueues();
            
            return services;
        }

        private static IServiceCollection RegisterQueues(this IServiceCollection services)
        {
            services.AddAWSService<IAmazonSQS>();
            services.AddScoped<IQueueService, QueueService>();

            return services;
        }

    }
}