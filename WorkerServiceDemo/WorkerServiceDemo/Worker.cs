using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JN.RabbitMQClient;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WorkerServiceDemo.HelperClasses;

namespace WorkerServiceDemo
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IRabbitMqConsumerService _rabbitConsumerService;
        private readonly IRabbitMqSenderService _rabbitSenderService;

        public Worker(ILogger<Worker> logger, IRabbitMqConsumerService rabbitConsumerService, IRabbitMqSenderService rabbitSenderService)
        {
            _logger = logger;
            _rabbitConsumerService = rabbitConsumerService;
            _rabbitSenderService = rabbitSenderService;

            _rabbitConsumerService.ServiceDescription = "Test Service";
            _rabbitConsumerService.ReceiveMessage += ProcessMessage;
            _rabbitConsumerService.ShutdownConsumer += ProcessShutdown;

            _rabbitSenderService.ServiceDescription = "Test Service sender";
        }

        private Task ProcessShutdown(string consumertag, ushort errorcode, string shutdowninitiator, string errormessage)
        {
            _logger.Info($"Shutdown '{consumertag}' | {errormessage}");
            return Task.CompletedTask;
        }

        private async Task<Constants.MessageProcessInstruction> ProcessMessage(string routingkeyorqueuename, string consumertag, long firsterrortimestamp, string exchange, string message)
        {
            _logger.Info($"Received message by '{consumertag}' | Content: {message}");

            _rabbitSenderService.Send(message);

            return await Task.FromResult(Constants.MessageProcessInstruction.OK);
        }

        /// <summary>
        /// Service start - executed when the service starts
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>

        public override Task StartAsync(CancellationToken stoppingToken)
        {
            _rabbitConsumerService.StartConsumers("Service consumer");

            _logger.LogInformation($"Starting consumers...");

            var details = _rabbitConsumerService.GetConsumerDetails();

            if (details != null)
            {
                foreach (var consumerInfo in details)
                {
                    _logger.LogInformation($"consumer '{consumerInfo.Name}' started at {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
                }

            }
            return base.StartAsync(stoppingToken);
        }

        /// <summary>
        /// Service stop - executed when the service stops
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        public override Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Service stopping...");
            return base.StopAsync(stoppingToken);
        }

        /// <summary>
        /// service execute - execute something
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Console.Out.WriteLineAsync($"Worker running at: {DateTimeOffset.Now}");

                await Task.Delay(30000, stoppingToken);
            }
        }
    }
}
