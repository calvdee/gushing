using Gushing.Events;
using Gushing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gushing.Reactors
{

    /// <summary>
    /// Implements the IAbstractMessageReactor interface and provides the OnDoneReacting
    /// method for firing the DoneReacting event.  Implementing classes must implement
    /// the React() and EndOfStream() methods <see cref="Gushing.Interfaces.IAsyncMessageReactor"/>.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to react to</typeparam>
    public abstract class AbstractMessageReactor<TMessage> : IMessageReactor<TMessage>
    {
        public virtual event EventHandler<DoneReactingArgs> DoneReacting;


        /// <summary>
        /// Fires the DoneReacting event
        /// </summary>
        /// <param name="args">The DoneReactingArgs to pass to the event handler</param>
        protected virtual void OnDoneReacting(DoneReactingArgs args)
        {
            if (DoneReacting != null) DoneReacting(this, args);
        }

        public abstract void React(TMessage message);
        public abstract void OnEndOfStream();
    }
}
