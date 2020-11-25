using System.Threading.Channels;

namespace Incremental.Common.Queue.Channel.Contract
{
    /// <summary>
    /// Reads from a queue.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IQueueReader<T>
    {
        /// <summary>
        /// Queue reader.
        /// </summary>
        ChannelReader<T> Reader { get; }
    }
}