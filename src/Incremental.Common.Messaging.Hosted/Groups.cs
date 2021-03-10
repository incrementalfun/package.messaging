using System;

namespace Incremental.Common.Queues
{
    /// <summary>
    ///     Group reference.
    /// </summary>
    internal static class Groups
    {
        /// <summary>
        ///     Default group.
        /// </summary>
        public static readonly string Default = nameof(Default);

        /// <summary>
        ///     Specific user group.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string User(Guid id)
        {
            return $"user:{id}";
        }
    }
}