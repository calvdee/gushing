using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gushing.Interfaces
{

    /// <summary>
    /// An IMessageWriter writes a message to a stream.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to write</typeparam>
    public interface IMessageWriter<TMessage>
    {
        /// <summary>
        /// Writes a message to the named stream.
        /// </summary>
        /// <param name="message">The message to write</param>
        /// <param name="stream">The name of the stream</param>
        void Write(TMessage message, String stream);
    }


}
