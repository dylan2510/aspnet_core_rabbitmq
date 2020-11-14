using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace hostedservice_rabbitmq.Services
{
    public class RabbitMQProducer : IHostedService
    {
        private IModel channel;
        private Timer _timer;

        public RabbitMQProducer()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = factory.CreateConnection();
            channel = connection.CreateModel();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            RegisterRabbitMQ();
            RegisterTimerPublishEvent();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void RegisterRabbitMQ()
        {
            // Create a Queue to publish messages to
            channel.QueueDeclare(
                queue: "rabbit",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
        }

        private void PublishEvent(object state)
        {
            // Create message body
            string message = $"{DateTime.Now} : Hello World!";
            var body = Encoding.UTF8.GetBytes(message);

            // Publish message to queue
            channel.BasicPublish(
                exchange: "",
                routingKey: "rabbit",
                basicProperties: null,
                body: body
                );
        }

        private void RegisterTimerPublishEvent()
        {
            // Publish to RabbitMQ Queue every 5 seconds
            _timer = new Timer(PublishEvent, null, 0, 5000);
        }
    }
}
