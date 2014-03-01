using Gushing.Events;
using Gushing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gushing.Readers
{


    /// <summary>
    /// Implements the IMessageReader interface and provides methods for firing
    /// the MessageRead, DoneReading, and EndOfStream events.  Implementing classes must implement
    /// the StartReading() and StopReading() methods <see cref="Gushing.Interfaces.IMessageReader"/>.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to react to</typeparam>
    public abstract class AbstractMessageReader<TMessage> : IMessageReader<TMessage>
    {
        /// <summary>
        /// Fired when the reactor is done reacting to messages
        /// </summary>
        public virtual event EventHandler<MessageReadArgs<TMessage>> MessageRead;
        public virtual event EventHandler<DoneReadingArgs> DoneReading;
        public virtual event EventHandler EndOfMessages;

        /// <summary>
        /// Fires the MessageRead event
        /// </summary>
        /// <param name="args">The MessageReadArgs to pass to the event handler</param>
        protected virtual void OnMessageRead(MessageReadArgs<TMessage> args)
        {
            if (MessageRead != null) MessageRead(this, args);
        }

        /// <summary>
        /// Fires the DoneReading event
        /// </summary>
        /// <param name="args">The DoneReadingArgs to pass to the event handler</param>
        protected virtual void OnDoneReading(DoneReadingArgs args)
        {
            if (DoneReading != null) DoneReading(this, args);
        }

        /// <summary>
        /// Fires the EndOfMessages event
        /// </summary>
        /// <param name="args">EventArgs to signal that there are no more messages to be read</param>
        protected virtual void OnEndOfMessages(EventArgs args)
        {
            if (EndOfMessages != null) EndOfMessages(this, args);
        }

        public abstract void StartReading(String stream);
        public abstract void StopReading();
    }


}
