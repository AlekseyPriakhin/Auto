using System.Text.Json;
using Auto.Messages;
using EasyNetQ;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using JsonSerializer = System.Text.Json.JsonSerializer;

public class Program
{
    private static readonly IConfigurationRoot config = ReadConfiguration();
    private static HubConnection hub;

    static async Task Main(string[] args)
    {
        
        hub = new HubConnectionBuilder().WithUrl(config.GetConnectionString("OwnerHub")).Build();
        await hub.StartAsync();
        Console.WriteLine("Соединение с хабом установлено");
        
        using var bus = RabbitHutch.CreateBus(config.GetConnectionString("RabbitMQ"));
        await bus.PubSub.SubscribeAsync<OwnerSecondMessage>("HubNotifier", HandleOwnerMessage);
        Console.WriteLine("Соединение с шиной установлено, ожидаю сообщений о владельцах");
        Console.ReadLine();
    }

    private static async void HandleOwnerMessage(OwnerSecondMessage message)
    {
        Console.WriteLine("Сообщение о владельце получено");
        var json = JsonSerializer.Serialize(message, JsonSettings());
        Console.WriteLine("Отправляю сообщение в хаб");
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