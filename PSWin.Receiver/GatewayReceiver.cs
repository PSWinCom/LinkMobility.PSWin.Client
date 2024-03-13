using LinkMobility.PSWin.Receiver.Exceptions;
using LinkMobility.PSWin.Receiver.Model;
using LinkMobility.PSWin.Receiver.Parsers;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace LinkMobility.GatewayReceiver
{
    public class GatewayReceiver
    {
        public delegate Task MoReceiver(MoMessage message);
        public delegate Task DrReceiver(DrMessage message);

        private const string XmlOkResponse = "<?xml version=\"1.0\"?><MSGLST><MSG><ID>1</ID><STATUS>OK</STATUS></MSG></MSGLST>";
        private readonly MoReceiver moReceiver;
        private readonly DrReceiver drReceiver;

        public GatewayReceiver(MoReceiver moReceiver, DrReceiver drReceiver)
        {
            this.moReceiver = moReceiver;
            this.drReceiver = drReceiver;
        }

        public async Task ReceiveMobileOriginatedMessageAsync(HttpContext context)
        {
            var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
            var result = await ReceiveMobileOriginatedMessageAsync(body);
            context.Response.StatusCode = (int)result.status;
            await HttpResponseWritingExtensions.WriteAsync(context.Response, result.responseBody);
        }

        public async Task<(HttpStatusCode status, string responseBody)> ReceiveMobileOriginatedMessageAsync(string requestBody)
        {
            if (moReceiver == null)
                throw new InvalidOperationException("MO receiver not configured");

            var (document, parseCode, parseMessage) = await GetDocumentFromBody(requestBody);
            if (document == null)
                return (parseCode, parseMessage);
            try
            {
                var momessage = MoParser.Parse(document);
                await moReceiver.Invoke(momessage);
                return (HttpStatusCode.OK, XmlOkResponse);
            }
            catch (MoParserException ex)
            {
                return (HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                return (HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public async Task ReceiveDeliveryReportAsync(HttpContext context)
        {
            var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
            var result = await ReceiveDeliveryReportAsync(body);
            context.Response.StatusCode = (int)result.status;
            await HttpResponseWritingExtensions.WriteAsync(context.Response, result.responseBody);
        }

        public async Task<(HttpStatusCode status, string responseBody)> ReceiveDeliveryReportAsync(string requestBody)
        {
            if (drReceiver == null)
                throw new InvalidOperationException("DR receiver not configured");

            var (document, parseCode, parseMessage) = await GetDocumentFromBody(requestBody);
            if (document == null)
                return (parseCode, parseMessage);
            try
            {
                var drmessage = DrParser.Parse(document);
                await drReceiver.Invoke(drmessage);
                return (HttpStatusCode.OK, XmlOkResponse);
            }
            catch (DrParserException ex)
            {
                return (HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                return (HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        private async Task<(XDocument document, HttpStatusCode code, string message)> GetDocumentFromBody(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return (null, HttpStatusCode.BadRequest, "Error: Content is empty");
            }
            try
            {
                return (XDocument.Parse(content), HttpStatusCode.OK, null);
            }
            catch (XmlException ex)
            {
                return (null, HttpStatusCode.BadRequest, $"XML is not well formed: {ex.Message}");
            }
        }
    }
}
