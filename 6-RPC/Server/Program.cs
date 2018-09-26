using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Server
{
    class Program
    {
        static void ConsumeLogsFromRabbitMQ(string queueName)
        {
            var connectionFactory = new ConnectionFactory() { HostName = "192.168.231.65", UserName = "vitor", Password = "churros", VirtualHost="ciclovida_dev" };

            using (var connection = connectionFactory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    var consumer = new EventingBasicConsumer(channel);

                    consumer.Received += ((model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        
                        var props = ea.BasicProperties;
                        var replyProps = channel.CreateBasicProperties();
                        // replyProps.CorrelationId = props.CorrelationId;

                        var defaultAnswer = "Default answer";

                        channel.BasicPublish("", props.ReplyTo, replyProps, Encoding.UTF8.GetBytes(defaultAnswer));

                        Console.WriteLine($"Received: {message}");
                        Console.WriteLine($"Answer: {defaultAnswer}");
                    });

                    channel.BasicConsume(queueName, true, consumer);

                    Console.WriteLine("Connection established...");
                    Console.ReadLine();
                }
            }
        }

        static void Main(string[] args)
        {
            ConsumeLogsFromRabbitMQ(args[0]);
        }
    }
}
