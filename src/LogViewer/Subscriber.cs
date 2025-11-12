using System;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LogViewer;

public class Subscriber : IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _exchangeName;

    public Subscriber(
        string hostName = "localhost",
        int port = 5672,
        string user = "guest",
        string pass = "guest",
        string exchangeName = "logs")
    {
        _exchangeName = exchangeName;

        var factory = new ConnectionFactory
        {
            HostName = hostName,
            Port = port,
            UserName = user,
            Password = pass,
            DispatchConsumersAsync = false
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(_exchangeName, ExchangeType.Fanout);

        var tempQueue = _channel.QueueDeclare("", durable: false, exclusive: true, autoDelete: true);
        var queueName = tempQueue.QueueName;

        _channel.QueueBind(queue: queueName, exchange: _exchangeName, routingKey: "");

        Console.WriteLine($"Conectado a RabbitMQ en {hostName}:{port}");
        Console.WriteLine($"Escuchando logs del exchange '{_exchangeName}'\n");

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());

            try
            {
                using var doc = JsonDocument.Parse(message);
                var root = doc.RootElement;

                var ts = root.GetProperty("Timestamp").GetString();
                var level = root.GetProperty("Level").GetString();
                var category = root.GetProperty("Category").GetString();
                var msg = root.GetProperty("Message").GetString();
                var ex = root.TryGetProperty("Exception", out var exEl) ? exEl.GetString() : null;

                Console.WriteLine($"[{ts}] {level} - {category}: {msg}");
                if (!string.IsNullOrEmpty(ex))
                    Console.WriteLine($"Exception: {ex}");
            }
            catch
            {
                Console.WriteLine(message);
            }
        };

        _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
    }

    public void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }
}
