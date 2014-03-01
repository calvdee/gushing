using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gushing.Events
{
    /// <summary>
    /// EventArgs for an event that is fired when the reactor has finished reacting to messages.
    /// Used by IMessageReactor and IAsyncMessageReactor.
    /// </summary>
    public class DoneReactingArgs : EventArgs
    {
        /// <summary>
        /// The number of messages that have been reacted to
        /// </summary>
        public readonly int NumberOfMessages;

        public DoneReactingArgs(int nMessages)
        {
            NumberOfMessages = nMessages;
        }
    }
}
