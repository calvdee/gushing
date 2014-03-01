using System;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;
using Gushing.Interfaces;

namespace Gushing.Writers
{
    public class MSMQWriter : IMessageWriter<String>
    {
        private Object m_Lock = new Object();
        private MessageQueue m_Queue;
        private readonly String m_FullQueueName;
        private readonly QueueAccessMode m_AccessMode = QueueAccessMode.Send;

        public MSMQWriter(String machineName)
        {
            if (machineName.Contains("."))
            {
                m_FullQueueName = "FormatName:DIRECT=TCP:";
            }
            else
            {
                m_FullQueueName = "FormatName:DIRECT=OS:";
            }

            m_FullQueueName += machineName;
            m_FullQueueName += @"\private$\";
        }

        public void Write(String message, String stream)
        {
            if (m_Queue == null)
            {
                lock (m_Lock)
                {
                    if (m_Queue == null)
                    {
                        // TODO: How do we handle connection exceptions when we are in another threading context?
                        m_Queue = new MessageQueue(m_FullQueueName + stream, false, false, m_AccessMode);
                        m_Queue.Formatter = new ActiveXMessageFormatter();
                    }
                }
            }

            // TODO: How do we handle message failure exceptions when we are in another threading context?
            m_Queue.Send(message, 0);
        }
    }
}
