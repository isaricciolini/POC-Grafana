using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using POC_Grafana.Models.ValueObjects;


namespace POC_Grafana.Configurations
{
    internal static class ConfiguracaoDI
    {
        public static IServiceCollection AddConfiguracaoDI(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AppSettings>(
                configuration.GetSection(nameof(AppSettings)));

            return services;
        }
    }
}
