using System;
using System.Collections.Generic;
using System.Text;
using JN.RabbitMQClient.Entities;
using Microsoft.Extensions.Configuration;

namespace WorkerServiceDemo.HelperClasses
{
    public static class SettingsHelper
    {
        public static BrokerConfig GetBrokerConfig(this IConfiguration configuration, string sectionName)
        {
            var section = configuration.GetSection(sectionName);

            byte.TryParse(section["TotalInstances"], out var totalInstances);

            var conf = new BrokerConfig()
            {
                Host = section["Host"],
                Port = Convert.ToInt16(section["Port"]),
                Password = section["Password"],
                VirtualHost = section["VirtualHost"],
                Username = section["Username"],
                Exchange = section["Exchange"],
                RoutingKeyOrQueueName = section["RoutingKeyOrQueueName"],
                TotalInstances = totalInstances

            };

            return conf;
        }
    }
}
