namespace Publisher
{
    using RabbitMQ.Client;
    using System;
    using System.Text;

    public class Program
    {
        static void PublishInRabbitMQ(string message, string routingKey)
        {
            var connectionFactory = new ConnectionFactory() { HostName = "sistema_77", UserName = "vitor", Password = "churros", VirtualHost ="ciclovida_dev" };

            using (var connection = connectionFactory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    var bytes = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish("logs", routingKey, true, null, bytes);

                    Console.WriteLine("Message successfully published.");
                }
            }
        }

        static void Main(string[] args)
        {
            PublishInRabbitMQ(args[0], args[1]);
        }
    }
}
