using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gushing.Events
{

    /// <summary>
    /// EventArgs for an event that is fired when a messsage has been read.
    /// Used by IMessageReactor and IAsyncMessageReactor.
    /// </summary>
    /// <typeparam name="TMessage">The type of message that was reacted to</typeparam>
    public class MessageReactedToArgs<TMessage> : EventArgs
    {
        /// <summary>
        /// The message that was reacted to
        /// </summary>
        public readonly TMessage Message;

        public MessageReactedToArgs(TMessage message)
        {
            Message = message;
        }
    }

}
