using Auto.Messages;
using EasyNetQ;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Auto.FineInfo;

public class Program
{
    private static readonly IConfigurationRoot config = ReadConfiguration();
    private static IBus bus;
    private static FineInfoService.FineInfoServiceClient client;
    public static async Task Main(string[] args)
    {
        using var channel = GrpcChannel.ForAddress(config.GetConnectionString("gRPC"));
             client = new FineInfoService.FineInfoServiceClient(channel);
        Console.WriteLine("Соединение с сервисом FineInfo установлено");
        bus = RabbitHutch.CreateBus(config.GetConnectionString("RabbitMQ"));
        Console.WriteLine("Соединение с шиной установлено, ожидаю сообщений о новых владельцах");
        await bus.PubSub.SubscribeAsync<NewOwnerMessage>("FineInfoClient", HandleNewOwnerMessage);
        Console.ReadLine();
    }

    private static async Task HandleNewOwnerMessage(NewOwnerMessage newOwnerMessage)
    {
        Console.WriteLine($"Получено сообщение о новом пользователе {newOwnerMessage.OwnerId}");
        var req = new FineRequest()
            {
                FirstName = newOwnerMessage.FirstName,
                SecondName = newOwnerMessage.SecondName,
                VehicleRegistration = newOwnerMessage.VehicleRegistration
            }; 
        
        
    var res = await client.GetFineInfoAsync(req);
    Console.WriteLine($"Получена информация о штрафах {res.FineStatus}");
    Console.WriteLine("Публикую новое сообщение");
    await bus.PubSub.PublishAsync(new OwnerSecondMessage()
    {
        FineStatus = res.FineStatus,
        OwnerId = newOwnerMessage.OwnerId,
        Mail = newOwnerMessage.Mail,
        FirstName = newOwnerMessage.FirstName,
        PhoneNumber = newOwnerMessage.PhoneNumber,
        SecondName = newOwnerMessage.SecondName,
        VehicleRegistration = newOwnerMessage.VehicleRegistration
    });
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
}
