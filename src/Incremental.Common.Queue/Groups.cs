﻿using System;

namespace Incremental.Common.Queue
{
    /// <summary>
    /// Group reference.
    /// </summary>
    public static class Groups
    {
        /// <summary>
        /// Default group.
        /// </summary>
        public static readonly string Default = nameof(Default);

        /// <summary>
        /// Specific user group.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string User(Guid id) => $"user:{id}";
    }
}