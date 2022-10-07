
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
        builder.AddIp("53246");

        //if (!isProduction)
        //{
            var credentials = new BasicAuthCredentials(
                "https://logs-prod3.grafana.net",
                "298716",
                "eyJrIjoiYjUyYjk1YWZmMzU1ZTA0NjI0N2FmMjFkZGRlNTQ4MjgwNDIxZjZlZiIsIm4iOiJhZG1pbiIsImlkIjo2MTQ5NDZ9"
                );

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

