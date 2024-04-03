using LinkMobility.PSWin.Receiver.Exceptions;
using LinkMobility.PSWin.Receiver.Model;
using LinkMobility.PSWin.Receiver.Parsers;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
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
        private static readonly Encoding defaultEncoding = Encoding.GetEncoding("ISO-8859-1");

        public GatewayReceiver(MoReceiver moReceiver, DrReceiver drReceiver)
        {
            this.moReceiver = moReceiver;
            this.drReceiver = drReceiver;
        }

        public async Task ReceiveMobileOriginatedMessageAsync(HttpContext context)
        {
            var body = await ReadBodyAsync(context.Request);
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
            var body = await ReadBodyAsync(context.Request);
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

        private static async Task<string> ReadBodyAsync(HttpRequest request)
        {
            Encoding encoding = GetEncoding(request, defaultEncoding);
            using (var reader = new StreamReader(request.Body, encoding))
            {
                return await reader.ReadToEndAsync();
            }
        }

        private static Encoding GetEncoding(HttpRequest request, Encoding defaultEncoding)
        {
            try
            {
                if (request.Headers.TryGetValue("Content-Type", out var values))
                {
                    if (values.Count > 0)
                    {
                        var contentType = values.First();
                        var charset = new ContentType(contentType).CharSet;
                        if (charset != null)
                            return Encoding.GetEncoding(charset);
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is NotSupportedException || ex is FormatException)
            {
                // Content-Type header malformed or encoding not supported.
                // Try default encoding instead.
            }
            return defaultEncoding;
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
