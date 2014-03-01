using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gushing.Events
{
    /// <summary>
    /// EventArgs for an event that is fired when the reader has finished reading messages.
    /// Used by the IMessageReader.
    /// </summary>
    public class DoneReadingArgs : EventArgs
    {
        /// <summary>
        /// The number of messages that have been read
        /// </summary>
        public readonly int NumberOfMessages;

        public DoneReadingArgs(int nMessages)
        {
            NumberOfMessages = nMessages;
        }
    }
}
