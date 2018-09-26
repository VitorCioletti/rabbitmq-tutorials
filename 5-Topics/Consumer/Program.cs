namespace Consumer
{
    using System;
    using RabbitMQ.Client;
    using System.Text;
    using RabbitMQ.Client.Events;

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
                        
                        Console.WriteLine($"Received: {message}");
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
