using LinkMobility.PSWin.Client;
using LinkMobility.PSWin.Client.Model;
using Microsoft.Extensions.Configuration;
using System.Reflection;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddUserSecrets(Assembly.GetExecutingAssembly())
    .Build();

var username = config["username"];
var password = config["password"];
var recipient = config["recipient"];

var client = new GatewayClient(username, password);

var result = await client.SendAsync(
    new Sms(
        senderNumber: "GCTEST",
        receiverNumber: recipient,
        text: "Hello, World!"));

if (result.IsStatusOk)
    Console.WriteLine("Message sent!");
else
    Console.WriteLine($"Message failed to send: {result.StatusText}");