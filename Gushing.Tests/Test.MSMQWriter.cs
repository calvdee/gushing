using Gushing.Interfaces;
using Gushing.Writers;
using NUnit.Framework;
using System;
using System.Linq;
using System.Messaging;

namespace Gushing.Tests
{
    [TestFixture]
    public class MSMQWriterTest
    {
        private String m_Stream;
        private MessageQueue m_Queue;
        private IMessageWriter<String> m_Writer;


        [TestFixtureSetUp]
        public void Setup()
        {
            
            m_Stream = "stream.invoices.linedata";
            
            m_Queue = GetPurgedMessageQueue(m_Stream);

            Assert.AreEqual(0, m_Queue.GetAllMessages().Count(), "Messages were not purged");

            m_Writer = new MSMQWriter(Environment.MachineName);
        }

        [TestCase]
        public void TestMSMQ_WritesToMessageQueue()
        {
            const String MSG_1 = @"\\test-app01\CDrive\e2e_test_xml\00023001010.XML";
            const String MSG_2 = @"\\test-app01\CDrive\e2e_test_xml\00023001011.XML";
            const String MSG_3 = @"\\test-app01\CDrive\e2e_test_xml\00023001012.XML";



            m_Writer.Write(MSG_1, m_Stream);
            m_Writer.Write(MSG_2, m_Stream);
            m_Writer.Write(MSG_3 , m_Stream);

            // Read back the messages
            Assert.AreEqual(MSG_1, m_Queue.Receive().Body.ToString());
            Assert.AreEqual(MSG_2, m_Queue.Receive().Body.ToString());
            Assert.AreEqual(MSG_3, m_Queue.Receive().Body.ToString());
        }

        public static MessageQueue GetPurgedMessageQueue(String name, QueueAccessMode mode = QueueAccessMode.ReceiveAndAdmin)
        {
            String queuePath = ".\\private$\\" + name;

            if (!MessageQueue.Exists(queuePath))
            {
                throw new InvalidOperationException("Queue " + queuePath + " doesn't exist");
            }

            {
                MessageQueue adminQueue = new MessageQueue(queuePath, QueueAccessMode.ReceiveAndAdmin);
                adminQueue.Purge();
            }

            MessageQueue queue = new MessageQueue(queuePath, mode);
            queue.Formatter = new ActiveXMessageFormatter();

            return queue;
        }
    }
}
