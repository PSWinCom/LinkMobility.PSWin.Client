using LinkMobility.PSWin.Client.Interfaces;
using LinkMobility.PSWin.Client.Model;
using LinkMobility.PSWin.Client.Transports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinkMobility.PSWin.Client
{
    /// <summary>
    /// 
    /// </summary>
    public class GatewayClient
    {
        private readonly ITransport transport;

        /// <summary>
        /// Initializes a client that sends messages using the default endpoint and transport.
        /// </summary>
        /// <param name="username">The username assigned to you by Link Mobility.</param>
        /// <param name="password">The password assigned to you by Link Mobility.</param>
        public GatewayClient(string username, string password) : this(new XmlTransport(username, password))
        {
        }

         /// <summary>
         /// Initializes a client that sends messages using the default transport.
         /// </summary>
         /// <param name="username">The username assigned to you by Link Mobility.</param>
         /// <param name="password">The password assigned to you by Link Mobility.</param>
         /// <param name="endpoint">The alternate XML endpoint to use.</param>
         public GatewayClient(string username, string password, Uri endpoint) : this(new XmlTransport(username, password, endpoint))
         {
         }

        /// <summary>
        /// Initialize a client using the given transport.
        /// </summary>
        /// <param name="transport">The transport to use.</param>
        public GatewayClient(ITransport transport)
        {
            this.transport = transport;
        }

        /// <summary>
        /// Send a single message.
        /// Using <see cref="SendAsync(IEnumerable{Sms}, string)"/> is more efficient if you need to send multiple messages.
        /// </summary>
        /// <seealso cref="SendAsync(IEnumerable{Sms}, string)"/>
        /// <param name="message">The message to send.</param>
        /// <param name="sessionData">See <see cref="SendAsync(IEnumerable{Sms}, string)"/>.</param>
        /// <returns>See <see cref="SendAsync(IEnumerable{Sms}, string)"/>.</returns>
        public async Task<MessageResult> SendAsync(Sms message, string sessionData = null)
        {
            return (await transport.SendAsync(new Sms[] { message }, sessionData)).Single();
        }

        /// <summary>
        /// Send multiple Sms messages to the endpoint and retreive the results.
        /// </summary>
        /// <seealso cref="SendAsync(Sms, string)"/>
        /// <param name="messages">
        ///     The messages to send.
        ///     Messages are automatically split into batches to avoid hitting the POST body size limit.
        ///     It is however not recommended that you send large message bulks with this client.
        ///     If you need to do so, please contact support instead.
        /// </param>
        /// <param name="sessionData">
        ///     A free text field that can be used to tag the session with customer specific data such as the application name, username, reference-id etc.
        ///     The maximum length is 200 characters.
        ///     Leave empty unless required.
        ///     If you need aggregated reports based on Session Data values​​, please contact support to get this enabled.
        /// </param>
        /// <returns>
        ///     The immediate response to the sent messages, in the same order as <paramref name="messages"/>.
        ///     The response does not indicate whether the message was actually delivered.
        ///     To get delivery status you need to have delivery reports enabled on your account,
        ///     and receive them with LinkMobility.PSWin.Receiver.
        /// </returns>
        public async Task<IEnumerable<MessageResult>> SendAsync(IEnumerable<Sms> messages, string sessionData = null)
        {
            return await transport.SendAsync(messages, sessionData);
        }
    }
}
