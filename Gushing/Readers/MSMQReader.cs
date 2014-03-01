using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;
using Gushing.Events;

namespace Gushing.Readers
{

    /// <summary>
    /// An implementation of an AbstractMessageReader that reads messages from
    /// a Microsoft Message Queue. Reading starts in a task and is thus awaited,
    /// allowing for asynchronous message processing.  If maxMessages is
    /// specified in the constructor, the reader will stop reading after the 
    /// message count has been reached.
    /// </summary>
    public class MSMQReader : AbstractMessageReader<String>
    {
        private MessageQueue m_Queue;
        private String m_FullQueueName;
        private readonly QueueAccessMode m_AccessMode = QueueAccessMode.Receive;
        private readonly int m_MaxMessages;
        private readonly Boolean m_AtLeastOneMessage;
        private int m_ReadMessages;
        private Boolean m_DoRead = true;

        public MSMQReader(String machineName, int maxMessages = 0, Boolean atLeastOneMessage = true)
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
            m_MaxMessages = maxMessages;
            m_AtLeastOneMessage = atLeastOneMessage;
        }

        public override void StartReading(String stream)
        {
            m_FullQueueName += stream;

            m_Queue = new MessageQueue(m_FullQueueName, false, false, m_AccessMode);
            m_Queue.Formatter = new ActiveXMessageFormatter();

            //m_Queue.ReceiveCompleted += new ReceiveCompletedEventHandler(ReceieveCompleted);

            ReadFromMQ();
        }

        public override void StopReading()
        {
            m_DoRead = false;
        }

        private async void ReadFromMQ()
        {
            await ReadFromMQAsync();
            OnDoneReading(new DoneReadingArgs(m_ReadMessages));
        }

        private Task ReadFromMQAsync()
        {   
            // TODO: Exception handling
            return Task.Run(() => {
                while (m_DoRead)
                {
                    try
                    {
                        String message = m_Queue.Receive(TimeSpan.FromSeconds(1)).Body.ToString();
                        m_ReadMessages++;
                        OnMessageRead(new MessageReadArgs<String>(message));
                    }
                    catch (Exception)
                    {
                        // If there is no message then we are out of messages -
                        // make sure we've read at least one.  This is a bad
                        // assumption for us to make so make this a ::TODO

                        if (m_AtLeastOneMessage && m_ReadMessages > 0)
                        {
                            OnEndOfMessages(EventArgs.Empty);
                            OnDoneReading(new DoneReadingArgs(m_ReadMessages));
                        }
                        else
                        {
                            OnEndOfMessages(EventArgs.Empty);
                            OnDoneReading(new DoneReadingArgs(m_ReadMessages));   
                        }
                        continue;
                    }
                    finally
                    {
                        if (m_MaxMessages != 0 && m_ReadMessages == m_MaxMessages)
                        {
                            m_DoRead = false;
                            OnDoneReading(new DoneReadingArgs(m_MaxMessages));
                        }
                    }
                }
            });
        }
    }


}
