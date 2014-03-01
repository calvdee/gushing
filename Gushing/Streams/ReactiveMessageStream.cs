using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Gushing.Readers;
using System.Reactive.Concurrency;
using System.Reactive.PlatformServices;
using Gushing.Reactors;
using Gushing.Interfaces;
using Gushing.Events;

namespace Gushing.Streams
{

    /// <summary>
    /// Implements an AbstractMessageStream using Microsoft's Rx library <see href="https://rx.codeplex.com/"/>.
    /// The stream is implemented using a Subject <see href="http://msdn.microsoft.com/en-us/library/hh242970(v=vs.103).aspx"/> which broadcasts messages
    /// to all Observers and can itself be an Observer.  IMeassageReaders
    /// that are added to the stream have their MessageRead event turned
    /// into an Observable (stream) <see href="http://msdn.microsoft.com/en-us/library/hh242978(v=vs.103).aspx"/> 
    /// to which the Subject subscribes.  Whenever a message is read, it will be sent to 
    /// the Subject which in turn sends it to it's Observers (reactors).
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public class ReactiveMessageStream<TMessage> : AbstractMessageStream<TMessage>
    {
        protected Subject<TMessage> m_Stream = null;
        protected Dictionary<int, IDisposable> m_Reactors = null;
        protected HashSet<int> m_Readers;
        
        public ReactiveMessageStream(String name) : base(name) 
        {
            m_Stream = new Subject<TMessage>();
        }
        
        public override void AddMessageReader(IMessageReader<TMessage> reader)
        {
            if(m_Readers == null)
            {
                m_Readers = new HashSet<int>();
            }
            else if(m_Readers.Contains(reader.GetHashCode()))
            {
                throw new InvalidOperationException("This reader is already reading messages.");
            }

            // Create a new observable sequence and hook it up to the observable stream
            IObservable<EventPattern<MessageReadArgs<TMessage>>> messages = Observable.FromEventPattern<MessageReadArgs<TMessage>>(reader, "MessageRead");

            // When events are fired, they will be fed into the bi-directional
            // observable stream implemented using a Subject<T>
            messages.Subscribe(msg => this.Write(msg.EventArgs.Message));

            reader.StartReading(Name);

            m_Readers.Add(reader.GetHashCode());
        }

        public override void AddMessageReactor(IMessageReactor<TMessage> reactor)
        {
            if (m_Reactors == null)
            {
                m_Reactors = new Dictionary<int, IDisposable>();
            }
            else if (m_Reactors.ContainsKey(reactor.GetHashCode()))
            {
                throw new InvalidOperationException("This reactor is already reacting to messages.");
            }

            // Hook up the reactor to the stream - every time the stream is written to,
            // the reactors's React() method will be called.
            IDisposable observer = m_Stream.Subscribe(
                m => reactor.React(m),
                () => reactor.OnEndOfStream());

            m_Reactors.Add(reactor.GetHashCode(), observer);
        }

        public override void AddMessageReactor(Action<TMessage> reactorFn)
        {
            IDisposable observer = m_Stream.Subscribe(m => reactorFn(m));
        }

        public override void AddAsyncMessageReactor(IAsyncMessageReactor<TMessage> reactor)
        {
            if (m_Reactors == null)
            {
                m_Reactors = new Dictionary<int, IDisposable>();
            }
            else if (m_Reactors.ContainsKey(reactor.GetHashCode()))
            {
                throw new InvalidOperationException("This reactor is already reacting to messages.");
            }

            // Hook up the reactor to the stream - every time the stream is written to,
            // the reactors's React() method will be called.
            IDisposable observer = m_Stream.Subscribe(
                async m => await reactor.React(m),
                () => reactor.OnEndOfStream());

            m_Reactors.Add(reactor.GetHashCode(), observer);
        }
        
        public override void Write(TMessage message)
        {
            m_Stream.OnNext(message);
        }
    }


}
