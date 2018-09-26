namespace Client
{
    using RabbitMQ.Client;
    using System.Text;
    using RabbitMQ.Client.Events;
    using System;
    using System.Collections.Concurrent;

    class Program
    {
        public class RPCClient
        {
            static string replyQueueName;

            static IConnection connection;

            static IModel channel;

            static IBasicProperties properties;

            static BlockingCollection<string> queueResponse = new BlockingCollection<string>();

            static EventingBasicConsumer consumer;

            public RPCClient()
            {
                var connectionFactory = new ConnectionFactory() 
                { 
                    HostName = "192.168.231.65", 
                    UserName = "vitor", 
                    Password = "churros", 
                    VirtualHost = "ciclovida_dev" 
                };

                connection = connectionFactory.CreateConnection();
                channel = connection.CreateModel();
                properties = channel.CreateBasicProperties();
                consumer = new EventingBasicConsumer(channel);

                replyQueueName = channel.QueueDeclare().QueueName;

                // properties.CorrelationId = Guid.NewGuid().ToString();
                properties.ReplyTo = replyQueueName;

                consumer.Received += (model, ea) =>
                {
                    // if (ea.BasicProperties.CorrelationId == properties.CorrelationId)
                        queueResponse.Add(Encoding.UTF8.GetString(ea.Body));
                };
            }

            public string Call(string message)
            {
                var bytes = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(
                    exchange: "chat",
                    routingKey: "user",
                    basicProperties: properties,
                    body: bytes
                );

                channel.BasicConsume(
                    consumer: consumer,
                    queue: replyQueueName,
                    autoAck: true
                );

                return queueResponse.Take();
            }

            public void Close() => connection.Close();
        }

        static void Main(string[] args)
        {
           var RPCClient = new RPCClient();
           var response = RPCClient.Call(args[0]);

           Console.WriteLine($"Response: {response}");

           RPCClient.Close();
        }
    }
}
