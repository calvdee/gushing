using Gushing.Events;
using Gushing.Interfaces;
using Gushing.Reactors;
using Gushing.Readers;
using Gushing.Streams;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Gushing.Tests
{

    [TestFixture]
    public class ReactiveMessageStreamTest
    {

        private const int EVENT_COUNT = 10;
        private const int SLEEP_TIME = 400;

        private static int m_ReactorCalls = 0;
        private static ManualResetEvent m_ReactorSignal = new ManualResetEvent(false);
        private IMessageStream<String> m_MessageStream = null;
        

        [TestFixtureSetUp]
        public void Setup()
        {
            m_MessageStream = new ReactiveMessageStream<String>("test-stream");
        }

        [TestCase]
        public void TestReactor_SynchronouslyReacts()
        {
            int calls = 0;

            // Setup the mock consumer 
            var mockReader = new Mock<AbstractMessageReader<String>>();

            mockReader.Setup(c => c.StartReading("test-stream"))
                        .Callback(() => { Thread.Sleep(SLEEP_TIME); });


            // Add the mock reader to the stream
            m_MessageStream.AddMessageReader(mockReader.Object);

            // Can't add twice
            Assert.Throws(typeof(InvalidOperationException), () => m_MessageStream.AddMessageReader(mockReader.Object));

            // Setup the mock reactor
            var mockReactor = new Mock<IMessageReactor<String>>();

            mockReactor.Setup(r => r.React("foomessage"))
                       .Callback(() => calls++);

            // Add the mock reactor to the stream
            m_MessageStream.AddMessageReactor(mockReactor.Object);

            // Can't add twice
            Assert.Throws(typeof(InvalidOperationException), () => m_MessageStream.AddMessageReactor(mockReactor.Object));

            Action raiseEvent = () =>
                mockReader.Raise(evt => evt.MessageRead += null, new MessageReadArgs<String>("foomessage"));

            // Simulate the reader reading messages
            Enumerable.Range(0, EVENT_COUNT).ToList().ForEach(i => raiseEvent());

            // There should have been 3 messages written to the stream
            Assert.AreEqual(EVENT_COUNT, calls);
        }

        [TestCase]
        public void TestReactor_AsyncReactorReacts()
        {
            // Setup the mock reader and hook it up to the stream
            var readerMock = GetAndSubscribeReaderMock();

            // Can't add twice
            //Assert.Throws(typeof(InvalidOperationException), () => m_EventStream.AddMessageReader(reader.Object));

            var reactor = new AsyncReactor();

            // Add the reactor
            m_MessageStream.AddAsyncMessageReactor(reactor);

            // Can't add twice
            //Assert.Throws(typeof(InvalidOperationException), () => m_EventStream.AddAsyncMessageReactor(reactor.Object));

            // Simulate reading 3 messages fromt he stream by firing events
            Enumerable.Range(0, EVENT_COUNT).ToList().ForEach(i => SendMockMessage(readerMock));


            m_ReactorSignal.WaitOne();

            Assert.AreEqual(EVENT_COUNT, m_ReactorCalls);
        }

        [TestCase]
        public void TestReactor_LambdaReactorReacts()
        {
            Boolean flag = false;
            Action<String> fn = (String s) => flag = true;
            
            // Setup the mock reader and hook it up to the stream
            var readerMock = GetAndSubscribeReaderMock();

            m_MessageStream.AddMessageReactor(fn);

            // Simulate reading a message
            SendMockMessage(readerMock);

            // The lambda should have been executed
            Assert.IsTrue(flag);
        }


        #region Helpers

        public Mock<AbstractMessageReader<String>> GetAndSubscribeReaderMock()
        {
            var mock = new Mock<AbstractMessageReader<String>>();
            mock.Setup(c => c.StartReading("test-stream"))
                        .Callback(() => { });
            m_MessageStream.AddMessageReader(mock.Object);
            return mock;
        }

        public void SendMockMessage(Mock<AbstractMessageReader<String>> mock)
        {
            Action raiseEvent = () => mock.Raise(evt => evt.MessageRead += null, new MessageReadArgs<String>("foomessage"));
            raiseEvent();
        }

        class AsyncReactor : AbstractAsyncMessageReactor<String>
        {

            public override Task React(string message)
            {
                return Task.Run(() =>
                {
                    m_ReactorCalls++;

                    Thread.Sleep(SLEEP_TIME);

                    if (m_ReactorCalls == EVENT_COUNT) m_ReactorSignal.Set();
                });
            }

            public override void OnEndOfStream()
            {
                throw new NotImplementedException();
            }
        }

        #endregion

    }

}
