using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gushing.Events
{

    /// <summary>
    /// EventArgs for an event that is fired when a messsage has been read.
    /// Used by the IMessageReader.
    /// </summary>
    /// <typeparam name="TMessage">The type of message that was reacted to</typeparam>
    public class MessageReadArgs<TMessage> : EventArgs
    {
        /// <summary>
        /// The message that was read
        /// </summary>
        public readonly TMessage Message;

        public MessageReadArgs(TMessage message)
        {
            Message = message;
        }
    }

}
