using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace Task_Management.Services.MessagingService
{
    public class MessageProducer : IMessageProducer
    {
        public void SendMessage<T>(T message)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost"
            };

            using var connection = factory.CreateConnection();

            using var channel = connection.CreateModel();

            channel.QueueDeclare("NotificationsQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);

            var messageSerialiser = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(messageSerialiser);

            channel.BasicPublish(exchange: "", routingKey: "NotificationsQueue", mandatory: false, basicProperties: null, body: body);
        }
    }
}