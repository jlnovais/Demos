using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace WorkerServiceDemo.HelperClasses
{
    public static class LoggerHelper
    {
        public static void Error<T>(this ILogger<T> logger, string message)
        {
            logger.LogError(message);
        }

        public static void Info<T>(this ILogger<T> logger, string message)
        {
            logger.LogInformation(message);
        }

        public static void Processed<T>(this ILogger<T> logger, string message)
        {
            logger.LogWarning(message);
        }

        public static void Received<T>(this ILogger<T> logger, string message)
        {
            logger.LogDebug(message);
        }

        //
    }
}