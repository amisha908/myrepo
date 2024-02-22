using Ecom.Services.EmailAPI.Services;
using Microsoft.AspNetCore.Connections;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Ecom.Services.EmailAPI.Messaging
{
    public class RabbitMQAuthConsumer : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private readonly string _rabbitMQConnectionString;
        private IConnection _connection;
        private IModel _channel;
        public RabbitMQAuthConsumer(IConfiguration configuration, EmailService emailService)
        {
            _configuration = configuration;
            _emailService = emailService;
            _rabbitMQConnectionString = _configuration.GetConnectionString("RabbitMQConnection");
           

        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(_rabbitMQConnectionString)
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(_configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue"), false, false, false, null);


            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (sender, e) =>
            {
                var content = Encoding.UTF8.GetString(e.Body.ToArray());
                String email = JsonConvert.DeserializeObject<string>(content);
                HandleMessage(email).GetAwaiter().GetResult();

                _channel.BasicAck(e.DeliveryTag, false);
            };
            _channel.BasicConsume(_configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue"), false, consumer);
            return Task.CompletedTask;
        }

        private async Task HandleMessage(string email)
        {
            _emailService.RegisterUserEmailAndLog(email).GetAwaiter().GetResult();
        }
    }
}
