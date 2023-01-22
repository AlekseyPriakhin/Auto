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
        //var amqp = "amqp://user:rabbitmq@localhost:5672";
        bus = RabbitHutch.CreateBus(config.GetConnectionString("RabbitMQ"));
        
        using var channel = GrpcChannel.ForAddress(config.GetConnectionString("gRPC"));
        client = new FineInfoService.FineInfoServiceClient(channel);
        
        await bus.PubSub.SubscribeAsync<NewOwnerMessage>("FineInfoClient", HandleNewOwnerMessage);
        Console.ReadLine();
    }

    private static async Task HandleNewOwnerMessage(NewOwnerMessage newOwnerMessage)
    {
        var req = new FineRequest()
            {
                FirstName = newOwnerMessage.FirstName,
                SecondName = newOwnerMessage.SecondName,
                VehicleRegistration = newOwnerMessage.VehicleRegistration
            }; 
        
    var res = await client.GetFineInfoAsync(req);
    //var ownerWithHash = new OwnerWithHashData(newOwnerMessage, res.Response);
    Console.WriteLine("Send second message");
    await bus.PubSub.PublishAsync(new SecondMessage()
    {
        FineStatus = res.FineStatus,
        Mail = newOwnerMessage.Mail,
        FirstName = newOwnerMessage.FirstName,
        PhoneNumber = newOwnerMessage.PhoneNumber,
        SecondName = newOwnerMessage.SecondName,
        VehicleRegistration = newOwnerMessage.VehicleRegistration
    });
    //await bus.PubSub.PublishAsync(ownerWithHash);
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
