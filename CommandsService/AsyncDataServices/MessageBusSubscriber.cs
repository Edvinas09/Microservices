
using System.Text;
using System.Threading.Tasks;
using CommandsService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandsService.AsyncDataServices
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IEventProcessor _eventProcessor;
        private IConnection? _connection;
        private IChannel? _channel;
        private string? _queueName;

        public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
        {
            _configuration = configuration;
            _eventProcessor = eventProcessor;
            InitializeMessageBusAsync().GetAwaiter().GetResult();
        }

        private async Task InitializeMessageBusAsync()
        {
            var bus = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQ:Host"] ?? "localhost",
                Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672")
            };
            _connection = await bus.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            await _channel.ExchangeDeclareAsync(exchange: "trigger", type: ExchangeType.Fanout);
            _queueName = (await _channel.QueueDeclareAsync()).QueueName;
            await _channel.QueueBindAsync(queue: _queueName, exchange: "trigger", routingKey: "");

            Console.WriteLine("--> Listening on the Message Bus...");

            _connection.ConnectionShutdownAsync += RabbitMQ_ConnectionShutdown;

        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            if (_channel != null && _queueName != null)
            {
                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.ReceivedAsync += (ModuleHandle, ea) =>
                {
                    Console.WriteLine("--> Event Received!");

                    var body = ea.Body.ToArray();
                    var notification = Encoding.UTF8.GetString(body.ToArray());
                    _eventProcessor.ProcessEvent(notification);
                    return Task.CompletedTask;
                };

                _channel.BasicConsumeAsync(queue: _queueName, autoAck: true, consumer: consumer);

            }
            return Task.CompletedTask;
        }

        private Task RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> RabbitMQ connection shutdown.");
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            if (_channel?.IsOpen == true)
            {
                _channel.CloseAsync();
                _connection?.CloseAsync();
            }
            base.Dispose();
        }
    }
}