using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using POC_Grafana.BackgroundServices;
using POC_Grafana.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeleconSistemas.Log.Extensions;

namespace POC_Grafana
{
    public class Startup
    {
        private readonly IHostEnvironment _currentEnvironment;
        private readonly IConfiguration _configuration;

        public Startup(IHostEnvironment currentEnvironment)
        {
            _currentEnvironment = currentEnvironment;
            //var pathToContentRoot = ObterCaminhoExecutavel();

            //Directory.SetCurrentDirectory(pathToContentRoot);

            var builder = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{_currentEnvironment.EnvironmentName}.json", optional: true)
                    .AddEnvironmentVariables();

            _configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddConfiguracaoDI(_configuration);
            services.AddHostedService<TestarLibLogsBackgroundService>();

            // services.AddInjecaoDependenciasConfig(_configuration);

            //chamar injeções de dependência usadas no seeder antes do seed de dados
            //services.AddSeedConfig(_currentEnvironment);//<<< SEED
            //chamar configs que dependem de dados depois do seed de dados
            services.AddLogConfig();
            
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            // Não roda inicialização do client HTTP durante testes
            if (env.IsStaging())
                return;

            app.UseTeleconRequestLogger();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
        }
    }
    
}
