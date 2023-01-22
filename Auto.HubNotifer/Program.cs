using System.Text.Json;
using Auto.Messages;
using EasyNetQ;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using JsonSerializer = System.Text.Json.JsonSerializer;

public class Program
{
    /*private static readonly IConfigurationRoot config = ReadConfiguration();
    private static HubConnection hubConnection;
    
    public static async Task Main(string[] args)
    {
        
         /*hubConnection = new HubConnectionBuilder().WithUrl(config.GetConnectionString("OwnerHub"))
                    .Build();
         await hubConnection.StartAsync();
         Console.WriteLine(hubConnection.State);#1#

        var amqp = "amqp://user:rabbitmq@localhost:5672";
        using var bus = RabbitHutch.CreateBus(amqp);
        await bus.PubSub.SubscribeAsync<OwnerWithHashData>("HubNotifier", M);
        Console.ReadLine();
    }
    
    
    
    
    private static IConfigurationRoot ReadConfiguration()
    {
        var basePath = Directory.GetParent(AppContext.BaseDirectory).FullName;
        Console.WriteLine(basePath);
        return new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();
    }

    private static async Task M(OwnerWithHashData message)
    {
        Console.WriteLine($"Received Hash -  {message.Hash}");
        var json = JsonSerializer.Serialize(message, Settings());
        await hubConnection.SendAsync("Notify", json);
    }
    private static JsonSerializerOptions Settings() =>
        new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };*/
    private static readonly IConfigurationRoot config = ReadConfiguration();
    const string SIGNALR_HUB_URL = "http://localhost:5265/ownerhub";
    private static HubConnection hub;

    static async Task Main(string[] args)
    {
        hub = new HubConnectionBuilder().WithUrl(config.GetConnectionString("OwnerHub")).Build();
        await hub.StartAsync();
        
        //var amqp = "amqp://user:rabbitmq@localhost:5672";
        
        using var bus = RabbitHutch.CreateBus(config.GetConnectionString("RabbitMQ"));
        
        await bus.PubSub.SubscribeAsync<SecondMessage>("HubNotifier", HandleNewVehicleMessage);
        Console.ReadLine();
    }

    private static async void HandleNewVehicleMessage(SecondMessage nvpm)
    {
        Console.WriteLine("Ok");
        var json = JsonSerializer.Serialize(nvpm, JsonSettings());
        await hub.SendAsync("Notify",
            json);
    }

    static JsonSerializerOptions JsonSettings() =>
        new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    
    private static IConfigurationRoot ReadConfiguration()
    {
        var basePath = Directory.GetParent(AppContext.BaseDirectory).FullName;
        Console.WriteLine(basePath);
        return new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();
    }
}