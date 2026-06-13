using System.Text;
using RabbitMQ.Client;

namespace backend_net.Services
{
    public class RabbitMQPublisher
    {
        private readonly string _hostname;

        // Ambil dari konfigurasi, default ke "localhost" jika jalan di laptop
        public RabbitMQPublisher(IConfiguration configuration)
        {
            _hostname = configuration["RabbitMQHost"] ?? "localhost"; 
        }

        public async Task SendMessage(string message)
        {
            var factory = new ConnectionFactory() { HostName = _hostname };
            // Lanjutan kode Anda...
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();
            
            await channel.QueueDeclareAsync(queue: "saham_queue", durable: true, exclusive: false, autoDelete: false);
            var body = Encoding.UTF8.GetBytes(message);

            await channel.BasicPublishAsync(exchange: "", routingKey: "saham_queue", body: body);
            Console.WriteLine($"Message sent to RabbitMQ: {message}");
        }
    }
}