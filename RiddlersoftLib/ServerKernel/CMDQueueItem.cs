using System;
using System.Collections.Generic;
using System.Text;

namespace ServerKernel
{
    /// <summary>
    /// an item that is queued up to be run on the next avalible oppotunity.
    /// </summary>
    public struct CMDQueueItem
    {
        /// <summary>
        /// the command line we are going to run
        /// </summary>
        public string CommandLine;

        /// <summary>
        /// the command action to run
        /// </summary>
        public Action<object> CommandAction;
    }
}
