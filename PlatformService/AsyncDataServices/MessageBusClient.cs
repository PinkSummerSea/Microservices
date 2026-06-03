using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PlatformService.Dtos;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient,IAsyncDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly ConnectionFactory _factory;
        private IConnection? _connection;

        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;
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
                Console.WriteLine("connected to messagebus");
                return _connection;
            }
            catch (Exception ex)
            {      
                throw new Exception($"cannot create rabbitmq connection: {ex.Message}");
            }
        }

        private async Task RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs @event)
        {
            Console.WriteLine("rabbitmq connection shutdown");
        }

        public async Task PublishNewPlatformAsync(PlatformPublishDto platformPublishDto)
        {
           var connection = await GetConnectionAsync();
           await using var channel = await connection.CreateChannelAsync();
           await channel.ExchangeDeclareAsync(exchange: "platform-events", type: ExchangeType.Fanout);

           var message = JsonSerializer.Serialize(platformPublishDto);
           Console.WriteLine("sending message...");
           await SendMessageAsync(channel, message);
        }

        private async Task SendMessageAsync (IChannel channel, string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            await channel.BasicPublishAsync(
                exchange: "platform-events",
                routingKey: "",
                body: body
            );

            Console.WriteLine($"message sent: {message}");
        }

        public async ValueTask DisposeAsync()
        {
            if(_connection is not null)
            {
                await _connection.DisposeAsync();
                Console.WriteLine("messagebus disposed");
            }
        }
    }
}