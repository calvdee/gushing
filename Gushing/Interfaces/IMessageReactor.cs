using Gushing.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gushing.Interfaces
{

    /// <summary>
    /// An IMessageReactor reacts to a message, and fires the DoneReacting event
    /// when it is done reacting to messages.  The React() method is called by 
    /// IMessageStream whenever a message is written to the stream 
    /// <see cref="Gushing.Interfaces.IMessageStream"/>
    /// </summary>
    /// <typeparam name="TMessage">The type of message this reactor will respond to.</typeparam>
    /// <seealso cref="Gushing.Interfaces.IMessageStream"/>
    public interface IMessageReactor<TMessage>
    {
        /// <summary>
        /// An event fired when the reactor has finished reacting
        /// </summary>
        event EventHandler<DoneReactingArgs> DoneReacting;

        /// <summary>
        /// Invoked when there is a message in the IMessageStream
        /// </summary>
        /// <param name="message">The message to react to</param>
        void React(TMessage message);

        /// <summary>
        /// Invoked by the IMessageStream when the end of the stream has been reached
        /// </summary>
        void OnEndOfStream();
    }


}
