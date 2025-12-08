using System;
using System.Threading;

namespace LogViewer;

class Program
{
    static void Main()
    {
        var topic = Environment.GetEnvironmentVariable("TOPIC");

        if (string.IsNullOrWhiteSpace(topic))
        {
            Console.WriteLine("No se ha definido TOPIC.");
            return;
        }

        using var subscriber = new Subscriber(RoutingKey: topic);

        Console.CancelKeyPress += (s, e) =>
        {
            Console.WriteLine("\nSaliendo...");
        };

        Console.WriteLine("Escuchando logs... Presiona CTRL+C para salir.");
        Thread.Sleep(Timeout.Infinite);
    }
}
