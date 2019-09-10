using Flattiverse;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentArea
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Connection connection = new Connection();

            connection.Received += received;
            connection.Disconnected += disconnected;

            await connection.Connect("Anonymous", "Password");

            while (true)
            {
                connection.Send(new Packet());
                connection.Flush();

                Thread.Sleep(100);
            }
        }

        private static void disconnected()
        {
            Console.WriteLine("Client disconnected.");
        }

        private static void received(List<Packet> packets)
        {
            foreach (Packet p in packets)
                Console.WriteLine(" * Packet Received: " + p.ToString());
        }
    }
}
