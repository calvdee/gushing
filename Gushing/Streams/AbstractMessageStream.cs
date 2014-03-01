using System;
using Gushing.Readers;
using Gushing.Reactors;
using Gushing.Interfaces;

namespace Gushing.Streams
{

    /// <summary>
    /// Implements the IMessageStream interface and provides a constructor to
    /// specify the stream name.  All other methods defined in IMessageStream
    /// must be implemented by the derived class.
    /// </summary>
    /// <typeparam name="TMessage">The type of message that can be written to the stream</typeparam>
    public abstract class AbstractMessageStream<TMessage> : IMessageStream<TMessage>
    {
        protected readonly String m_Name;
        
        /// <summary>
        /// The stream name.  This value is immutable.
        /// </summary>
        public String Name { get { return m_Name; } }

        public AbstractMessageStream(String name)
        {
            m_Name = name;
        }

        public abstract void AddMessageReader(IMessageReader<TMessage> reader);
        public abstract void AddMessageReactor(IMessageReactor<TMessage> reactor);
        public abstract void AddMessageReactor(Action<TMessage> reactorFn);
        public abstract void AddAsyncMessageReactor(IAsyncMessageReactor<TMessage> reactor);
        public abstract void Write(TMessage message);
    }

}
