using Gushing.Events;
using Gushing.Interfaces;
using Gushing.Readers;
using NUnit.Framework;
using System;
using System.Linq;
using System.Messaging;
using System.Threading;


namespace Gushing.Tests
{
    [TestFixture]
    public class MSMQReaderTest
    {
        private String m_Stream;
        private MessageQueue m_Queue;
        private const int N_MESSAGES = 3;
        static int unboundedReceives = 0;
        
        [TestFixtureSetUp]
        public void Setup()
        {
            m_Stream = "invoice.lineitem";
            
            m_Queue = MSMQWriterTest.GetPurgedMessageQueue(m_Stream, QueueAccessMode.SendAndReceive);
            

            Assert.AreEqual(0, m_Queue.GetAllMessages().Count(), "Messages were not purged");
        }

        [TestCase]
        public void TestMSMQ_ReadsFromMessageQueue()
        {
            ManualResetEvent signal = new ManualResetEvent(false);
            IMessageReader<String> reader = new MSMQReader(Environment.MachineName);

            reader.MessageRead += new EventHandler<MessageReadArgs<String>>((o, e) => 
            {
                if (++unboundedReceives == N_MESSAGES) signal.Set();
            });

            reader.StartReading(m_Stream);

            // Publish some messages to the queue
            for (int i = 1; i <= N_MESSAGES; ++i) m_Queue.Send(i);

            signal.WaitOne();

            // All the messages should have been consumed.
            Assert.AreEqual(0, m_Queue.GetAllMessages().Length);
        }

        [TestCase]
        public void TestMSMQ_FiresEndOfMessagesEvent()
        {
            ManualResetEvent signal = new ManualResetEvent(false);
            IMessageReader<String> reader = new MSMQReader(Environment.MachineName);

            reader.EndOfMessages += (o, e) => signal.Set();

            // Publish some messages to the queue
            for (int i = 1; i <= N_MESSAGES; ++i) m_Queue.Send(i);

            reader.StartReading(m_Stream);

            signal.WaitOne();

            // All the messages should have been consumed.
            Assert.AreEqual(0, m_Queue.GetAllMessages().Length);
        }

        [TestCase]
        public void TestMSMQ_FiresDoneReadingEvent()
        {
            ManualResetEvent signal = new ManualResetEvent(false);
            IMessageReader<String> reader = new MSMQReader(Environment.MachineName, N_MESSAGES);
            
            int messagesRead = 0;
            int boundedReads = 0;

            reader.MessageRead += new EventHandler<MessageReadArgs<String>>((o, e) =>
            {
                ++boundedReads;
            });

            reader.DoneReading += new EventHandler<DoneReadingArgs>((o, e) =>
            {
                messagesRead = e.NumberOfMessages;
                signal.Set();
            });

            reader.StartReading(m_Stream);

            // Write some messages to the queue
            for (int i = 1; i <= N_MESSAGES; ++i) m_Queue.Send(i);

            // Wait for the reader to stop reading
            signal.WaitOne();


            // All the messages should have been read.
            Assert.AreEqual(0, m_Queue.GetAllMessages().Length);

            // The number of messages from the DoneReading event should be equal
            // to the number of messages we wrote.
            Assert.AreEqual(N_MESSAGES, messagesRead);
        }
    
    }
}
