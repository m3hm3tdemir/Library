﻿using System;
using System.Text;
using BookingLibrary.Domain.Core.Messaging;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace BookingLibrary.Infrastructure.Messaging.RabbitMQ
{
    public class RabbitMQCommandPublisher : ICommandPublisher
    {
        private readonly IConnection connection;
        private readonly IModel channel;

        public RabbitMQCommandPublisher(string uri)
        {
            var factory = new ConnectionFactory() { Uri = new Uri(uri) };
            this.connection = factory.CreateConnection();
            this.channel = connection.CreateModel();
        }

        public void Dispose()
        {
            this.channel.Dispose();
            this.connection.Dispose();
        }

        public void Publish<ICommand>(ICommand command)
        {
            var json = JsonConvert.SerializeObject(command, Formatting.Indented, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
            var bytes = Encoding.UTF8.GetBytes(json);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: "", routingKey: "commandQueue", basicProperties: properties, body: bytes);
        }
    }
}
