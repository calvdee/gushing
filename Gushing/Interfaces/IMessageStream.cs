using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gushing.Interfaces
{

    /// <summary>
    /// An IMessageStream is composed of readers and reactors.  The messages that are
    /// read by the readers are added to the event stream via subscription to the
    /// MessageRead event <seealso cref="Gushing.Interfaces.MessageRead"/>.
    /// Once messages have been written to the stream, they are processes (reacted to)
    /// by reactors.
    /// </summary>
    /// <typeparam name="TMessage">The type of messages this stream can consume</typeparam>
    public interface IMessageStream<TMessage>
    {
        /// <summary>
        /// Directly writes a message to the stream
        /// </summary>
        /// <param name="message">The message to write</param>
        void Write(TMessage message);

        /// <summary>
        /// Adds an IMessageReader reader that fires an event whenever it reads a message
        /// </summary>
        /// <param name="reader">An IMessageReader that reads messages</param>
        void AddMessageReader(IMessageReader<TMessage> reader);

        /// <summary>
        /// Adds an IMessageReactor to react to messages in the stream.  The IMessageStream
        /// will call the reactor's React() method with a message
        /// </summary>
        /// <param name="reactor">An IMessageReactor reactor</param>
        void AddMessageReactor(IMessageReactor<TMessage> reactor);

        /// <summary>
        /// Adds an anonymous function to be executed whenever a message has been 
        /// written to the stream
        /// </summary>
        /// <param name="reactorFn">The action to execute when a message has been read</param>
        void AddMessageReactor(Action<TMessage> reactorFn);

        /// <summary>
        /// Adds an IAsyncMessageReactor to asynchronously react to messages in the stream.
        /// The reactor's React() method may be awaited allowing for asynchronous execution.
        /// </summary>
        /// <param name="reactor">An IMessageReactor reactor</param>
        void AddAsyncMessageReactor(IAsyncMessageReactor<TMessage> reactor);
    }

}
