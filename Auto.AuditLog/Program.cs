using EasyNetQ;
using Microsoft.Extensions.Configuration;
using Auto.Messages;

namespace Auto.AuditLog
{

    class Program
    {
        private static readonly IConfigurationRoot config = ReadConfiguration();

        private const string SUBSCRIBER_ID = "Auto.AuditLog";

        static async Task Main(string[] args)
        {
            var amqp = config.GetConnectionString("RabbitMQ");
            using var bus = RabbitHutch.CreateBus(amqp);
            Console.WriteLine("Подключено, ожидаю сообщения о новых владельцах");
            await bus.PubSub.SubscribeAsync<NewOwnerMessage>(SUBSCRIBER_ID, HandleNewOwnerMessage);
            Console.ReadKey();
        }

        private static void HandleNewOwnerMessage(NewOwnerMessage message)
        {
            Console.WriteLine("Сообщение о новом владельце получено");
            Console.WriteLine($"{message.FirstName}\n{message.SecondName}\n{message.OwnerId}\n{message.VehicleRegistration}" );
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
