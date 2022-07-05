using LinkMobility.GatewayReceiver;
using LinkMobility.PSWin.Receiver.Model;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var receiver = new GatewayReceiver(
    async (mo) => await PrintMo(mo),
    async (dr) => await PrintDr(dr));

app.MapGet("/", () => "Try POSTing XML from the Wiki to /mo and /dr");
app.MapPost("/mo", receiver.ReceiveMobileOriginatedMessageAsync);
app.MapPost("/dr", receiver.ReceiveDeliveryReportAsync);

app.Run();


async Task PrintMo(MoMessage message)
{
    Console.ForegroundColor = ConsoleColor.Magenta;
    await Console.Out.WriteLineAsync($"Received MO from {message.Sender} with the text \"{message.Text}\"");
    Console.ResetColor();
}

async Task PrintDr(DrMessage message)
{
    Console.ForegroundColor = ConsoleColor.Magenta;
    if (message.IsDelivered)
        await Console.Out.WriteLineAsync($"Message with reference {message.Reference} was delivered successfully.");
    else
        await Console.Out.WriteLineAsync($"Message with reference {message.Reference} was not delivered. Status: {message.State}");
    Console.ResetColor();
}