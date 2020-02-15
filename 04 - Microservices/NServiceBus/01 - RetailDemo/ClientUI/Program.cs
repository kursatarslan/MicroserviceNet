﻿using Commands;
using NServiceBus;
using NServiceBus.Logging;
using System;
using System.Threading.Tasks;

namespace ClientUI
{
    public class Program
    {
        static ILog log = LogManager.GetLogger<Program>();
        static void Main(string[] args)
        {
            AsyncMain().GetAwaiter().GetResult();
        }

        static async Task AsyncMain()
        {
            Console.Title = "ClientUI";
            var endpointConfiguration = new EndpointConfiguration("HandleMessages");
            var transport = endpointConfiguration.UseTransport<LearningTransport>();

            var endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            await RunLoop(endpointInstance).ConfigureAwait(false);

            //Console.WriteLine("Press Enter to exit...");
            //Console.ReadLine();

            await endpointInstance.Stop().ConfigureAwait(false);
        }

        static async Task RunLoop(IEndpointInstance endpointInstance)
        {
            while (true)
            {
                log.Info("Press 'P' to place an order, or 'Q' to quit.");
                var key = Console.ReadKey();
                Console.WriteLine();

                switch (key.Key)
                {
                    case ConsoleKey.P:
                        // Instantiate the command
                        var command = new PlaceOrderCommand
                        {
                            OrderId = Guid.NewGuid().ToString()
                        };

                        // Send the command to the local endpoint
                        log.Info($"Sending PlaceOrder command, OrderId = {command.OrderId}");
                        await endpointInstance.SendLocal(command)
                            .ConfigureAwait(false);

                        break;

                    case ConsoleKey.Q:
                        return;

                    default:
                        log.Info("Unknown input. Please try again.");
                        break;
                }
            }
        }
    }
}
