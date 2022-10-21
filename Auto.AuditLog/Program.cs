using EasyNetQ;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Auto.Messages;

namespace Auto.AuditLog
{

    class Program
    {
        private static readonly IConfigurationRoot config = ReadConfiguration();

        private const string SUBSCRIBER_ID = "Auto.AuditLog";

        static async Task Main(string[] args)
        {
            using var bus = RabbitHutch.CreateBus("host=localhost;timeout=120");
            Console.WriteLine("Connected! Listening for NewOwnerMessage messages.");
            await bus.PubSub.SubscribeAsync<NewOwnerMessage>(SUBSCRIBER_ID, HandleNewOwnerMessage);
            Console.ReadKey();
        }

        private static void HandleNewOwnerMessage(NewOwnerMessage message)
        {
            Console.WriteLine($"{message.Name} {message.SecondName} {message.OwnerId} {message.VehicleRegistration} {message.CreatedAtUtc}" );
        }

        private static IConfigurationRoot ReadConfiguration()
        {
            var basePath = Directory.GetParent(AppContext.BaseDirectory).FullName;
            return new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
