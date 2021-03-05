namespace Incremental.Common.Queues
{
    /// <summary>
    /// Queue endpoints.
    /// </summary>
    public static class QueuesEndpoints
    {
        /// <summary>
        /// Service Queue.
        /// </summary>
        public static readonly string Services = "https://sqs.eu-west-1.amazonaws.com/***REMOVED***/incremental_services.fifo";
        
        /// <summary>
        /// Statistics Queue.
        /// </summary>
        public static readonly string Statistics = "https://sqs.eu-west-1.amazonaws.com/***REMOVED***/incremental_statistics.fifo";
    }
}