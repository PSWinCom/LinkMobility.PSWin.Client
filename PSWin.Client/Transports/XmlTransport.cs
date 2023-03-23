using LinkMobility.PSWin.Client.Extensions;
using LinkMobility.PSWin.Client.Interfaces;
using LinkMobility.PSWin.Client.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LinkMobility.PSWin.Client.Transports
{
    /// <summary>
    /// Implementation of the PSWin XML API.
    /// </summary>
    public class XmlTransport : ITransport
    {
        private static readonly Uri DefaultEndpoint = new Uri("https://xml.pswin.com");
        private const uint BatchSize = 100;

        private readonly Uri endpoint;
        private string username;
        private string password;
        private Lazy<HttpClient> client;

        /// <summary>
        /// Initialize the transport with an alternate endpoint.
        /// This is useful if for example Link Mobility has assigned you an account on the test system.
        /// Note that it must be an XML endpoint, not the SOAP or Simple HTTP endpoints that PSWin also provides.
        /// </summary>
        /// <param name="username">The username assigned to the account on the given <paramref name="endpoint"/>.</param>
        /// <param name="password">The password assigned to the account on the given <paramref name="endpoint"/>.</param>
        /// <param name="endpoint">The alternate XML endpoint to use.</param>
        /// <param name="messageHandler">Messagehandler used when sending xml</param>
        public XmlTransport(string username, string password, Uri endpoint, HttpMessageHandler messageHandler)
        {
            this.username = username;
            this.password = password;
            this.endpoint = endpoint;
            this.client = new Lazy<HttpClient>(() => new HttpClient(messageHandler));
        }

        /// <summary>
        /// Initialize the transport with an alternate endpoint.
        /// This is useful if for example Link Mobility has assigned you an account on the test system.
        /// Note that it must be an XML endpoint, not the SOAP or Simple HTTP endpoints that PSWin also provides.
        /// </summary>
        /// <param name="username">The username assigned to the account on the given <paramref name="endpoint"/>.</param>
        /// <param name="password">The password assigned to the account on the given <paramref name="endpoint"/>.</param>
        /// <param name="endpoint">The alternate XML endpoint to use.</param>
        public XmlTransport(string username, string password, Uri endpoint) : this(username, password, endpoint, new HttpClientHandler())
        {
        }

        /// <summary>
        /// Initialize the transport with the default endpoint.
        /// The default endpoint is connected to the production system that sends real text messages and bills your account accordingly.
        /// </summary>
        /// <param name="username">The username assigned to you by Link Mobility.</param>
        /// <param name="password">The password assigned to you by Link Mobility.</param>
        public XmlTransport(string username, string password) : this(username, password, DefaultEndpoint)
        {
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<MessageResult>> SendAsync(IEnumerable<Sms> messages, string sessionData = null)
        {
            var messageBatches = messages.Batch(BatchSize);
            var results = new List<MessageResult>();
            foreach (var messageBatch in messageBatches)
            {
                var batchResults = await SendBatchAsync(messageBatch, sessionData);
                results.AddRange(batchResults);
            }
            return results;
        }

        private async Task<IEnumerable<MessageResult>> SendBatchAsync(Sms[] messageBatch, string sessionData)
        {
            var payload = BuildBatchPayload(sessionData, messageBatch);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = endpoint,
                Content = new StringContent(payload.Declaration.ToString() + payload.ToString(), Encoding.UTF8, "text/xml"),
            };

            var response = await client.Value.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                throw new SendMessageException($"Sending failed because Gateway endpoint returned {(int)response.StatusCode} {response.ReasonPhrase}");

            var responseContent = await response.Content.ReadAsStringAsync();
            return GetSendResult(XDocument.Parse(responseContent), messageBatch);
        }

        private XDocument BuildBatchPayload(string sessionData, IEnumerable<Sms> messages)
        {
            var session = new XElement("SESSION");
            session.Add(new XElement("CLIENT", username));
            session.Add(new XElement("PW", password));
            if (!string.IsNullOrEmpty(sessionData))
                session.Add(new XElement("SD", sessionData));
            session.Add(BuildMessageList(messages));
            
            return new XDocument(
                    declaration: new XDeclaration("1.0", "iso-8859-1", null),
                    content: session);
        }

        private XElement BuildMessageList(IEnumerable<Sms> messages)
        {
            return new XElement("MSGLST", messages.Select(BuildMessage));
        }

        private XElement BuildMessage(Sms msg, int index)
        {
            var text = msg.Text;
            if (msg.Type == MessageType.Unicode)
                text = Ucs2HexEncode(text);

            var result = new XElement("MSG");
            result.Add(new XElement("ID", index + 1));
            result.Add(new XElement("TEXT", text));
            result.Add(new XElement("SND", msg.SenderNumber));
            result.Add(new XElement("RCV", msg.ReceiverNumber));

            if (msg.Payment != null)
            {
                result.Add(new XElement("TARIFF", msg.Payment.Tariff));
                if (msg.Payment.AgeRestriction.HasValue)
                    result.Add(new XElement("AGELIMIT", msg.Payment.AgeRestriction.Value.ToString("0")));
                if (!string.IsNullOrEmpty(msg.Payment.ServiceCode))
                    result.Add(new XElement("SERVICECODE", msg.Payment.ServiceCode));
            }
            
            if (msg.Type.HasValue)
                result.Add(new XElement("OP", msg.Type.Value.ToString("D")));
            if (msg.TimeToLive.HasValue)
                result.Add(new XElement("TTL", msg.TimeToLive.Value.TotalMinutes.ToString("0")));
            if (msg.DeliveryTime.HasValue)
                result.Add(new XElement("DELIVERYTIME", msg.DeliveryTime.Value.ToString("yyyyMMddHHmm")));
            if (msg.Replace.HasValue)
                result.Add(new XElement("REPLACE", msg.Replace.Value.ToString("D")));
            if (msg.IsFlashMessage)
                result.Add(new XElement("CLASS", "0"));

            return result;
        }

        private static string Ucs2HexEncode(string text)
        {
            // BigEndianUnicode a.k.a. UTF-16BE is a superset of UCS2.
            var ucs2Bytes = Encoding.BigEndianUnicode.GetBytes(text);
            var hexBuilder = new StringBuilder();
            foreach (var b in ucs2Bytes)
                hexBuilder.AppendFormat("{0:x2}", b);
            return hexBuilder.ToString();
        }

        private static IEnumerable<MessageResult> GetSendResult(XDocument response, Sms[] messages)
        {
            var logon = response.Descendants("LOGON").FirstOrDefault()?.Value;
            var reason = response.Descendants("REASON").FirstOrDefault()?.Value;
            if (logon == "OK")
            {
                var results = new MessageResult[messages.Length];
                foreach (var el in response.Descendants("MSG"))
                {
                    var index = int.Parse(el.Element("ID").Value) - 1;
                    results[index] = new MessageResult(
                        el.Element("REF")?.Value,
                        el.Element("STATUS")?.Value == "OK",
                        el.Element("INFO")?.Value,
                        messages[index]
                    );
                }
                return results;
            }
            else
            {
                return messages.Select(m => new MessageResult(null, false, reason, m));
            }
        }
    }
}
