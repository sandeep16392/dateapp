using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace DateApp.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .CreateLogger();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseSerilog((ctx, provider, loggerConfig)=>
            {
                loggerConfig
                .ReadFrom.Configuration(ctx.Configuration)
                //.WriteTo.EventCollector(ctx.Configuration["SplunkUrl"], ctx.Configuration["Splunkkey"]);
                .WriteTo.Seq(ctx.Configuration["SeqLog"]);        //For SeqLog
            })
            .ConfigureAppConfiguration((context, config) =>
            {
                var builtConfig = config.Build();
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                var isDevelopment = environment == Environments.Development;
                if(!isDevelopment)
                    config.AddAzureKeyVault(
                        $"https://{builtConfig["KeyVault:Vault"]}.vault.azure.net/",
                        builtConfig["KeyVault:ClientId"],
                        builtConfig["KeyVault:ClientSecret"],
                        new DefaultKeyVaultSecretManager());
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}
