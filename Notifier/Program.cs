using System.Text.Json;
using Auto.Messages;
using EasyNetQ;
using Microsoft.AspNetCore.SignalR.Client;
using JsonSerializer = System.Text.Json.JsonSerializer;

internal class Program
{
    const string SIGNALR_HUB_URL = "http://localhost:5265/ownerhub";
    private static HubConnection hub;

    static async Task Main(string[] args)
    {
        hub = new HubConnectionBuilder().WithUrl(SIGNALR_HUB_URL).Build();
        await hub.StartAsync();
        Console.WriteLine("Hub started!");
        Console.WriteLine("Press any key to send a message (Ctrl-C to quit)");
        var amqp = "amqp://user:rabbitmq@localhost:5672";
        using var bus = RabbitHutch.CreateBus(amqp);
        Console.WriteLine("Connected to bus! Listening for newVehicleMessages");
        var subscriberId = $"Auto.Notifier@{Environment.MachineName}";
        await bus.PubSub.SubscribeAsync<SecondMessage>(subscriberId, HandleNewVehicleMessage);
        Console.ReadLine();
    }

    private static async void HandleNewVehicleMessage(SecondMessage nvpm)
    {
        Console.WriteLine($"{nvpm.I}");
        /*var csvRow =
            $"{nvpm.Price} {nvpm.CurrencyCode} : {nvpm.Registration},{nvpm.ManufacturerName}," +
            $"{nvpm.ModelName},{nvpm.Year},{nvpm.Color},{nvpm.CreatedAt:O}";*/
        //Console.WriteLine(csvRow);
        var json = JsonSerializer.Serialize(nvpm, JsonSettings());
        await hub.SendAsync("Notify",
            json);
    }

    static JsonSerializerOptions JsonSettings() =>
        new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
}