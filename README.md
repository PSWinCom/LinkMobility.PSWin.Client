# LinkMobility.PSWin.Client

## About the Project

Send SMS and receive mobile-originated messages and delivery reports using the LINK Mobility PSWin gateway.

This is an implementation of the API described on the [PSWin Wiki](https://wiki.pswin.com/).

A PSWin account from [LINK Mobility](https://www.linkmobility.com/) is required.

## Getting Started

The project is avalable as two separate NuGet packages:
- `LinkMobility.PSWin.Client` for sending SMS.
- `LinkMobility.PSWin.Receiver` for receiving mobile-originated messages and delivery reports.

The packages target .NET Standard 2.0 and can therefore be used in .NET Framework and .NET Core projects.
See the [compatibility table](https://docs.microsoft.com/en-us/dotnet/standard/net-standard?tabs=net-standard-2-0#tabpanel_1_net-standard-2-0) for more information.

### Installation

```
PM> Install-Package LinkMobility.PSWin.Client
PM> Install-Package LinkMobility.PSWin.Receiver
```

## Quick Start

In addition to the below examples, you can find .NET Core example projects in this GitHub repository:
- `Example.Client`: Sending SMS with `LinkMobility.PSWin.Client`.
- `Example.Receiver`: Receiving mobile-orignated messages and delivery reports.

### Send an SMS

```C#
var client = new GatewayClient(username, password);
var result = await client.SendAsync(
    new Sms(
        senderNumber: "GCTEST",
        receiverNumber: "4700000000",
        text: "Hello, World!"));
Console.WriteLine(result.IsStatusOk ? "Message sent!" : "Message not sent!");
```

### Receive a Mobile-Originated Message

```C#
var receiver = new GatewayReceiver(async (mo) => Console.WriteLine(mo.Text), null);

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.MapPost("/", receiver.ReceiveMobileOriginatedMessageAsync);
app.Run();
```

## License

This project is licensed under the MIT No Attribution license.
See [./LICENSE.txt](LICENSE.txt) for the license text.
