using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using Newtonsoft.Json;
using Middleware.RabbitMQ.Interface;

namespace Middleware.RabbitMQ.Service
{
    public class RabbitMQProducer:IRabbitMQProducer
    {
        private readonly IConfiguration _config;
        public RabbitMQProducer(IConfiguration config)
        {
            _config = config;
        }

        public void PublishMessage(string email, string token)
        {
            var factory = new ConnectionFactory
            {
                HostName = _config["RabbitMQ:Host"],
                UserName = _config["RabbitMQ:Username"],
                Password = _config["RabbitMQ:Password"],
            };
            var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(
                queue: _config["RabbitMQ:QueueName"],
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
            var message = new
            {
                Email = email,
                Token = token
            };
            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(
                exchange: "",
                routingKey: _config["RabbitMQ:QueueName"],
                basicProperties: null,
                body: body
            );
        }

    }
}
