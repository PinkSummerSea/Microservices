using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandService.AsyncDataServices
{
    public class MessageBusSubscriber : BackgroundService,IAsyncDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly IEventProcessor _eventProcessor;
        private readonly ConnectionFactory _factory;
        private IConnection? _connection;

        public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
        {
            _configuration = configuration;
            _eventProcessor = eventProcessor;
            _factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQHost"]!,
                Port = int.Parse(_configuration["RabbitMQPort"]!),
                UserName="guest",
                Password="guest"
            };
        }

        private async Task<IConnection> GetConnectionAsync()
        {
            if(_connection is { IsOpen: true }){
                return _connection;
            }
            try
            {
                _connection = await _factory.CreateConnectionAsync();
                _connection.ConnectionShutdownAsync += RabbitMQ_ConnectionShutdown;
                Console.WriteLine("command service connected to messagebus");
                return _connection;
            }
            catch (Exception ex)
            {
                throw new Exception($"command service: cannot create rabbitmq connection: {ex.Message}");
            }

        }

        private async Task RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs @event)
        {
            Console.WriteLine("command service rabbitmq connection shutdown");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var connection = await GetConnectionAsync();
            await using var channel = await connection.CreateChannelAsync(cancellationToken:stoppingToken);
            await channel.ExchangeDeclareAsync(exchange: "platform-events", type: ExchangeType.Fanout,cancellationToken:stoppingToken);
            var queue = await channel.QueueDeclareAsync(cancellationToken:stoppingToken);
            await channel.QueueBindAsync(queue: queue.QueueName, exchange: "platform-events", routingKey: "",cancellationToken:stoppingToken);
            Console.WriteLine("command service listening on the message bus...");

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                Console.WriteLine("event received!");

                var body = ea.Body;
                var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

                _eventProcessor.ProcessEvent(notificationMessage);
                await Task.CompletedTask;
            };

            await channel.BasicConsumeAsync(
                queue: queue.QueueName,
                autoAck: true,
                consumer: consumer,
                cancellationToken: stoppingToken
            );

            try
            {
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Message bus listener stopping...");
            }
        }

        public async ValueTask DisposeAsync()
        {
            if(_connection is not null)
            {
                await _connection.DisposeAsync();
                Console.WriteLine("command service messagebus disposed");
            }
            GC.SuppressFinalize(this);
        }
    }
}