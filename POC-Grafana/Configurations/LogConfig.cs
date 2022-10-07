using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spring.Core.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeleconSistemas.Log;
using TeleconSistemas.Log.Enums;

namespace POC_Grafana.Configurations
{
    public static class LogConfig
    {
        public static IServiceCollection AddLogConfig(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();

            ILoggerFactory loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            ILogger logger = loggerFactory.CreateLogger("POC-Grafana");
            TeleconLog.SetNivel(NivelLog.Information);

            return services;
        }
    }
}
