
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using POC_Grafana;
using POC_Grafana.Models.ValueObjects;
using Serilog.Sinks.Loki;
using Serilog.Sinks.SystemConsole.Themes;
using System.Diagnostics;
using System.Runtime.InteropServices;
using TeleconSistemas.Log;
using TeleconSistemas.Log.Extensions;

public static class Program
{
    
    public static async Task Main(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        bool isProduction = environment == Environments.Production || environment == null;


        if (isProduction)
        {
            var processModule = Process.GetCurrentProcess().MainModule;
            if (processModule != null)
            {
                var pathToExe = processModule.FileName;
                var pathToContentRoot = Path.GetDirectoryName(pathToExe);
                Directory.SetCurrentDirectory(pathToContentRoot);
            }
        }

        try
        {
            ConfigurarLogs(isProduction);
            await CreateHostBuilder(args).Build().RunAsync().ConfigureAwait(false);
        }
        catch (Exception fatalException)
        {
            Serilog.Log.Fatal("Erro fatal na API do Torus PDV. Exception: {exception}", fatalException);
            throw;
        }
        finally
        {
            TeleconLog.Finalizar();
        }
    }

    private static void ConfigurarLogs(bool isProduction)
    {
        bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        ITeleconLogBuilder builder = TeleconLog.CreateDefaultBuilder();
        builder.AddPrefixoComNivelDeLogExpecifico("/HubHardware", Serilog.Events.LogEventLevel.Verbose);
        builder.AddOverrideNivelMinimoDeLog("Hangfire", Serilog.Events.LogEventLevel.Warning);
        builder.AddConsole(SystemConsoleTheme.Literate);
        builder.AddIp("Adiciona label com número de IP");

        //if (!isProduction)
        //{
            var credentials = new BasicAuthCredentials(
                "[link]",
                "[key]",
                "[token]"
                );

        // Adiciona ao grafana apenas logs com a palavra GRAFANA no início
            builder.AddLoki(
                credentials,
                logEvent => logEvent.RenderMessage().StartsWith("GRAFANA", StringComparison.InvariantCultureIgnoreCase)
                );
            
        //}

        if (isWindows)
        {
            builder.AddEventLog();
        }

        builder.AddPropertyTimeStampCompleto();
        builder.Build();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseTeleconLog()
                .ConfigureWebHostDefaults(webBuilder =>
{
                    webBuilder.UseStartup<Startup>();
                });
}

