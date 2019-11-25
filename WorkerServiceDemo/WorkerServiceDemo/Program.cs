using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JN.RabbitMQClient;
using JN.RabbitMQClient.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using WorkerServiceDemo.HelperClasses;

namespace WorkerServiceDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var build = CreateHostBuilder(args).Build();

            var logger = build.Services.GetService<ILogger<Program>>();

            logger.Info("Starting service...");

            build.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)

                // for windows services: install package Microsoft.Extensions.Hosting.WindowsServices and use .UseWindowsService()
                // for linux:            install package Microsoft.Extensions.Hosting.Systemd and use .UseSystemd()
                .UseSystemd()

                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging((loggingBuilder) =>
                    {
                        // configure Logging with NLog
                        //loggingBuilder.ClearProviders();
                        loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                        loggingBuilder.AddNLog(hostContext.Configuration);
                    });

                    services.AddSingleton<IRabbitMqConsumerService, RabbitMqConsumerService>();
                    services.AddSingleton<IRabbitMqSenderService, RabbitMqSenderService>();
                    services.AddSingleton<IBrokerConfigSender>(
                        hostContext.Configuration.GetBrokerConfig("BrokerConfigSender"));
                    services.AddSingleton<IBrokerConfigConsumers>(
                        hostContext.Configuration.GetBrokerConfig("BrokerConfigConsumers"));

                    services.AddHostedService<Worker>();
                });
    }
}
