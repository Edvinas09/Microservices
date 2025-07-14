using System.Threading.Tasks;
using PlatformService.Dtos;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection? _connection;
        private readonly IChannel? _channel;

        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQHost"] ?? "localhost",
                Port = int.Parse(_configuration["RabbitMQPort"] ?? "5672")
            };
            try
            {
                _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
                _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
                _channel.ExchangeDeclareAsync(exchange: "trigger", type: ExchangeType.Fanout).GetAwaiter().GetResult();
                // Subscribe to the connection shutdown event
                _connection.ConnectionShutdownAsync += RabbitMQ_ConnectionShutdownAsync;

                Console.WriteLine("--> Connected to the message bus");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not connect to the message bus: {ex.Message}");

            }
        }
        public async Task PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
        {
            var message = System.Text.Json.JsonSerializer.Serialize(platformPublishedDto);
            int maxRetries = 2;
            int delayMs = 1000;
            for (int attempt = 1; attempt < maxRetries; attempt++)
            {
                if (_connection?.IsOpen == true)
                {
                    Console.WriteLine("--> RabbitMQ connection is open, sending message...");
                    await SendMessage(message);
                    return;

                }
                else
                {
                    Console.WriteLine($"--> RabbitMQ connection is closed, retrying in {delayMs}ms..., attempt {attempt}/{maxRetries}");
                    await Task.Delay(delayMs);
                }
            }
            Console.WriteLine("--> RabbitMQ connection is still closed after retries, not sending message");
        }

        private async Task SendMessage(string message)
        {
            var body = System.Text.Encoding.UTF8.GetBytes(message);
            if (_channel != null)
            {
                await _channel.BasicPublishAsync(
                    exchange: "trigger",
                    routingKey: "",
                    body: body
                );
            }
            Console.WriteLine($"--> Message sent: {message}");
        }

        public void Dispose()
        {
            Console.WriteLine("--> Disposing MessageBusClient");
            if (_channel?.IsOpen == true)
            {
                _channel.CloseAsync();
                _connection?.CloseAsync();
            }
        }

        private Task RabbitMQ_ConnectionShutdownAsync(object? sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> RabbitMQ connection shutdown");
            return Task.CompletedTask;
        }
    }
}