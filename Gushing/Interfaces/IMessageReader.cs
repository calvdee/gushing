using Gushing.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gushing.Interfaces
{

    /// <summary>
    /// An IMessageReader reads messages from a message stream and provides
    /// events for when messages are read and when the reader is done 
    /// receiving messages.  Both DoneReading and EndOfMessages should be
    /// fired when there are no more messages to read.  There is no guarantee
    /// to the order in which these events are fired.
    /// </summary>
    /// <typeparam name="TMessage">The type of message that the reader will read</typeparam>
    public interface IMessageReader<TMessage>
    {
        /// <summary>
        /// Fired when a message has been read
        /// </summary>
        event EventHandler<MessageReadArgs<TMessage>> MessageRead;

        /// <summary>
        /// Fired when the reader is done reading messages
        /// </summary>
        event EventHandler<DoneReadingArgs> DoneReading;

        /// <summary>
        /// Fired when there are no more message to read
        /// </summary>
        event EventHandler EndOfMessages;

        /// <summary>
        /// Causes the reader to start reading messages from the stream
        /// </summary>
        /// <param name="stream">The name of the stream</param>
        void StartReading(String stream);

        /// <summary>
        /// Forces the reader to stop reading messages from the stream
        /// </summary>
        void StopReading();
    }

}
