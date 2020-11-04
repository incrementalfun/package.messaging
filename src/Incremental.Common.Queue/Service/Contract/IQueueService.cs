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
        /// <param name="queue"></param>
        /// <param name="event"></param>
        /// <param name="groupId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task Send(string queue, IExternalEvent @event, string groupId, CancellationToken cancellationToken = default);
        
    }
}