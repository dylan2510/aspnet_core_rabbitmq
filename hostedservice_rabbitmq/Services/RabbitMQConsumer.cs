using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace hostedservice_rabbitmq.Services
{
    public class RabbitMQConsumer : IHostedService
    {

        private IConnection connection;
        private IModel channel;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            RegisterRabbitMQConsumer();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            DeRegister();
            return Task.CompletedTask;
        }

        private void RegisterRabbitMQConsumer()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            // Declare the queue channel for listening
            channel.QueueDeclare(queue: "rabbit",
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

            // Create consumer
            var consumer = new EventingBasicConsumer(channel);

            // Consumer handler for message recieved
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Debug.WriteLine($"Received Message: {message}");
            };

            // Attach handler to queue
            channel.BasicConsume(queue: "rabbit",
                             autoAck: true,
                             consumer: consumer);

        }

        public void DeRegister()
        {
            connection.Close();
        }
    }
}
