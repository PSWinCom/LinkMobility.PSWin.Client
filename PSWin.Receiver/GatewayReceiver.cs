using LinkMobility.PSWin.Receiver.Exceptions;
using LinkMobility.PSWin.Receiver.Model;
using LinkMobility.PSWin.Receiver.Parsers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
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
        private readonly ILogger log = NullLogger.Instance;
        private static readonly Encoding defaultEncoding = Encoding.GetEncoding("ISO-8859-1");
        
        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayReceiver"/> class with the specified logger, MO receiver, and DR receiver.
        /// </summary>
        /// <param name="logger">
        ///     Logger will receive debug messages for successful operations,
        ///     warnings for bad client requests,
        ///     and errors for invalid configuration and unexpected exceptions.
        /// </param>
        /// <param name="moReceiver">The delegate that will be invoked when a mobile originated message is received.</param>
        /// <param name="drReceiver">The delegate that will be invoked when a delivery report is received.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="logger"/> is null.</exception>
        public GatewayReceiver(ILogger logger, MoReceiver moReceiver, DrReceiver drReceiver)
            : this(moReceiver, drReceiver)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            this.log = logger;
            log.LogInformation(nameof(GatewayReceiver) + " logging initialized");
        }

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
            {
                log.LogError("MO receiver not configured");
                throw new InvalidOperationException("MO receiver not configured");
            }

            var (document, parseCode, parseMessage) = await GetDocumentFromBody(requestBody);
            if (document == null)
            {
                log.LogWarning("MO endpoint returning {StatusCode}: {ErrorMessage}", (int)parseCode, parseMessage);
                return (parseCode, parseMessage);
            }
            try
            {
                var momessage = MoParser.Parse(document);
                await moReceiver.Invoke(momessage);
                log.LogDebug("MO endpoint returning {StatusCode}: Message processed", (int)HttpStatusCode.OK);
                return (HttpStatusCode.OK, XmlOkResponse);
            }
            catch (MoParserException ex)
            {
                log.LogWarning("MO endpoint returning {StatusCode}: MO parser encounered error: {ErrorMessage}", (int)HttpStatusCode.BadRequest, ex.Message);
                return (HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "MO endpoint returning {StatusCode}: Unexpected error", (int)HttpStatusCode.InternalServerError);
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
            {
                log.LogError("DR receiver not configured");
                throw new InvalidOperationException("DR receiver not configured");
            }

            var (document, parseCode, parseMessage) = await GetDocumentFromBody(requestBody);
            if (document == null)
            {
                log.LogWarning("DR endpoint returning {StatusCode}: {ErrorMessage}", (int)parseCode, parseMessage);
                return (parseCode, parseMessage);
            }
            try
            {
                var drmessage = DrParser.Parse(document);
                await drReceiver.Invoke(drmessage);
                log.LogDebug("DR endpoint returning {StatusCode}: Report processed", (int)HttpStatusCode.OK);
                return (HttpStatusCode.OK, XmlOkResponse);
            }
            catch (DrParserException ex)
            {
                log.LogWarning("DR endpoint returning {StatusCode}: DR parser encounered error: {ErrorMessage}", (int)HttpStatusCode.BadRequest, ex.Message);
                return (HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "DR endpoint returning {StatusCode}: Unexpected error", (int)HttpStatusCode.InternalServerError);
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
                return (null, HttpStatusCode.BadRequest, "Request content is empty");
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
