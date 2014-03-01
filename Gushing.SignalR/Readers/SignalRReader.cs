using Gushing.Events;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Client;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Gushing.Readers
{


    /// <summary>
    /// Implements an AbstractMessageReader using a SignalR Hub <see href="http://www.asp.net/signalr/overview/signalr-20/getting-started-with-signalr-20/introduction-to-signalr"/>
    /// as the backing message source.  Whenever the server method is called
    /// with a message of type TModel, the server will broadcast the message
    /// to this reader.  StartReading() will block until a connection is 
    /// established or the connection attempt times out in which case an
    /// exception must be handled by the caller.
    /// </summary>
    /// <typeparam name="TModel">The SignalR model which defines the message</typeparam>
    public class SignalrReader<TModel> : AbstractMessageReader<TModel>
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly String m_ServerMethod;
        private readonly String m_Endpoint;

        /// <summary>
        /// Creates a SignalR reader that listens for messages when the
        /// specified server method is called.
        /// </summary>
        /// <param name="endpoint">The SignalR endpoint</param>
        /// <param name="method">The SignalR method </param>
        /// <param name="maxMessages">The upper bound of messages before StopReading() is called</param>
        public SignalrReader(String endpoint, String method, int maxMessages = 0)
        {
            
            m_ServerMethod = method;
            m_Endpoint = endpoint;
        }

        public override void StartReading(String stream)
        {
            logger.Info("Attempting to connect to SignalR endpoint at {0}", m_Endpoint);

            try
            {
                var connection = new HubConnection(m_Endpoint);
                IHubProxy proxy = connection.CreateHubProxy(stream);

                connection.Start().Wait();

                proxy.On<TModel>(m_ServerMethod, x => OnMessageRead(new MessageReadArgs<TModel>(x)));
            }
            catch (Exception ex)
            {
                throw ex;
            }

            // ::TODO: Add logic for bounded reads
        }

        public override void StopReading()
        {
            throw new NotImplementedException();
        }
    }


}
