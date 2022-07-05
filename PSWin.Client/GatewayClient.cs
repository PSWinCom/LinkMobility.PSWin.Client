using LinkMobility.PSWin.Client.Interfaces;
using LinkMobility.PSWin.Client.Model;
using LinkMobility.PSWin.Client.Transports;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinkMobility.PSWin.Client
{
    public class GatewayClient
    {
        public const uint BatchSize = 150;

        private readonly ITransport transport;

        public GatewayClient(string username, string password) : this(new XmlTransport(username, password))
        {
        }

        public GatewayClient(ITransport transport)
        {
            this.transport = transport;
        }

        public async Task<MessageResult> SendAsync(Sms message, string sessionData = null)
        {
            return (await transport.SendAsync(new Sms[] { message }, sessionData)).Single();
        }

        public async Task<IEnumerable<MessageResult>> SendAsync(IEnumerable<Sms> messages, string sessionData = null)
        {
            return await transport.SendAsync(messages, sessionData);
        }
    }
}
