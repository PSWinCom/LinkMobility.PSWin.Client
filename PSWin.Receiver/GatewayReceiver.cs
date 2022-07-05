using LinkMobility.PSWin.Receiver.Exceptions;
using LinkMobility.PSWin.Receiver.Interfaces;
using LinkMobility.PSWin.Receiver.Model;
using LinkMobility.PSWin.Receiver.Parsers;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace LinkMobility.GatewayReceiver
{
    public class GatewayReceiver
    {
        private readonly IMoReceiver moReceiver;
        private readonly IDrReceiver drReceiver;

        public GatewayReceiver(IMoReceiver moReceiver, IDrReceiver drReceiver)
        {
            this.moReceiver = moReceiver;
            this.drReceiver = drReceiver;
        }

        public async Task<HttpResponseMessage> ReceiveMobileOriginatedMessage(HttpRequestMessage message)
        {
            if (moReceiver == null)
                throw new InvalidOperationException("MO receiver not configured");

            var (document, requestError) = await DocumentFromMessage(message);
            if (requestError != null)
                return requestError;
            try
            {
                var momessage = MoParser.Parse(document);
                await moReceiver.ReceiveAsync(momessage);
                return Success();
            }
            catch (MoParserException ex)
            {
                return BadReqeust(ex.Message);
            }
            catch (Exception ex)
            {
                return ServerError(ex.Message);
            }
        }
        public async Task<HttpResponseMessage> ReceiveDeliveryReport(HttpRequestMessage message)
        {
            if (drReceiver == null)
                throw new InvalidOperationException("DR receiver not configured");

            var (document, requestError) = await DocumentFromMessage(message);
            if (requestError != null)
                return requestError;
            try
            {
                var drMessage = DrParser.Parse(document);
                await drReceiver.ReceiveAsync(drMessage);
                return Success();
            }
            catch (MoParserException ex)
            {
                return BadReqeust(ex.Message);
            }
            catch (Exception ex)
            {
                return ServerError(ex.Message);
            }
        }

        private HttpResponseMessage Success()
        {
            return new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                ReasonPhrase = "OK",
            };
        }

        private HttpResponseMessage BadReqeust(string message)
        {
            return new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.BadRequest,
                ReasonPhrase = "Bad Request",
                Content = new StringContent(message),
            };
        }

        private HttpResponseMessage ServerError(string message)
        {
            return new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                ReasonPhrase = "Internal Server Error",
                Content = new StringContent(message),
            };
        }

        private async Task<(XDocument, HttpResponseMessage)> DocumentFromMessage(HttpRequestMessage message)
        {
            if (message.Method != HttpMethod.Post)
            {
                return (null, new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.MethodNotAllowed,
                    ReasonPhrase = "Expected POST",
                });
            }
            var content = await message.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(content))
            {
                return (null, BadReqeust("Error: Content is empty"));
            }
            try
            {
                return (XDocument.Parse(content), null);
            }
            catch (XmlException ex)
            {
                return (null, BadReqeust($"XML is not well formed: {ex.Message}"));
            }
        }
    }
}
