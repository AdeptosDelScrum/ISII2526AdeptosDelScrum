using System;
using System.Threading;

namespace LogViewer;

class Program
{
    static void Main()
    {
        using var subscriber = new Subscriber();

        Console.CancelKeyPress += (s, e) =>
        {
            Console.WriteLine("\nSaliendo...");
        };

        Console.WriteLine("Escuchando logs... Presiona CTRL+C para salir.");
        Thread.Sleep(Timeout.Infinite);
    }
}
