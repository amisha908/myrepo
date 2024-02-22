using Microsoft.AspNetCore.Connections;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Ecom.Services.AuthAPI.RabbitMQSender
{
    public class RabbitMQAuthMessageSender : IRabbitMQAuthMessageSender
    {

        private readonly string _rabbitMQConnectionString;
        private IConfiguration _configuration;
        private IConnection _connection;

        public RabbitMQAuthMessageSender(IConfiguration configuration)
        {
            _configuration = configuration;
            _rabbitMQConnectionString = _configuration.GetConnectionString("RabbitMQConnection");
        }
        public void SendMessage(object message, string queueName)
        {
            if (ConnectionExists())
            {
                using var channel = _connection.CreateModel();
                channel.QueueDeclare(queueName, false, false, false, null);
                var json = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(json);
                channel.BasicPublish(exchange: "", routingKey: queueName, null, body: body);
            }


        }

        private void CreateConnection()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    Uri = new Uri(_rabbitMQConnectionString)
                };

                _connection = factory.CreateConnection();
            }
            catch (Exception ex)
            {

            }
        }

        private bool ConnectionExists()
        {
            if (_connection != null)
            {
                return true;
            }
            CreateConnection();
            return true;
        }
    }
}
