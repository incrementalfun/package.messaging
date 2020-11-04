using System.Threading;
using System.Threading.Tasks;
using Incremental.Common.Sourcing.Events.Contract;

namespace Incremental.Common.Queue.Service.Contract
{
    /// <summary>
    /// Queue service.
    /// </summary>
    public interface IQueueService
    {
        /// <summary>
        /// Sends an event to the queues.
        /// </summary>
        /// <param name="event"></param>
        /// <param name="queue"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task Send(IExternalEvent @event, string queue, CancellationToken cancellationToken = default);
        
    }
}