using Gushing.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gushing.Interfaces
{


    /// <summary>
    /// An IAsyncMessageReactor asynchronously reacts to a message, returning
    /// a Task representing a computation on the message that will finish in the future.
    /// IAsyncMessageReactors may be added to an IMessageStream and the stream will await the
    /// React(), passing it a message.
    /// </summary>
    /// <typeparam name="TMessage">The type of message this reactor will respond to.</typeparam>
    /// <seealso cref="Gushing.Interfaces.IMessageStream"/>
    public interface IAsyncMessageReactor<TMessage>
    {
        /// <summary>
        /// An event fired when the reactor has finished reacting
        /// </summary>
        event EventHandler<DoneReactingArgs> DoneReacting;

        /// <summary>
        /// Invoked and awaited when there is a message in the IMessageStream
        /// </summary>
        /// <param name="message">The message to react to</param>
        /// <returns>The computation wrapped in a Task</returns>
        Task React(TMessage message);

        /// <summary>
        /// Invoked by the IMessageStream when the end of the stream has been reached
        /// </summary>
        void OnEndOfStream();
    }


}
