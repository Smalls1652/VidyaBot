using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VidyaBot.App.Services;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using System.Reflection;
using OpenTelemetry.Logs;
using OpenTelemetry.Exporter;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Azure.Monitor.OpenTelemetry.Exporter;

var hostBuilder = Host.CreateApplicationBuilder(args);

hostBuilder.Configuration
    .AddEnvironmentVariables()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile(
        path: "appsettings.json",
        optional: true,
        reloadOnChange: true
    )
    .AddJsonFile(
        path: $"appsettings.{hostBuilder.Environment.EnvironmentName}.json",
        optional: true,
        reloadOnChange: true
    );

hostBuilder.Logging.ClearProviders();

hostBuilder.Logging
    .AddOpenTelemetry(logging =>
    {
        logging.IncludeScopes = true;
        logging.IncludeFormattedMessage = true;

        var resourceBuilder = ResourceBuilder
            .CreateDefault()
            .AddService(Assembly.GetExecutingAssembly().GetName().Name!);

        logging
            .SetResourceBuilder(resourceBuilder)
            .AddConsoleExporter()
            .AddOtlpExporter();

        if (hostBuilder.Configuration.GetValue<string>("APPLICATIONINSIGHTS_CONNECTION_STRING") is not null)
        {
            logging.AddAzureMonitorLogExporter(options =>
            {
                options.ConnectionString = hostBuilder.Configuration.GetValue<string>("APPLICATIONINSIGHTS_CONNECTION_STRING");
            });
        }
    });

hostBuilder.Services
    .AddOpenTelemetry()
    .ConfigureResource(resourceBuilder => resourceBuilder.AddService(Assembly.GetExecutingAssembly().GetName().Name!))
    .WithMetrics(metrics =>
    {
        metrics
            .AddRuntimeInstrumentation()
            .AddHttpClientInstrumentation();

        metrics.AddOtlpExporter();

        if (hostBuilder.Configuration.GetValue<string>("APPLICATIONINSIGHTS_CONNECTION_STRING") is not null)
        {
            metrics.AddAzureMonitorMetricExporter(options =>
            {
                options.ConnectionString = hostBuilder.Configuration.GetValue<string>("APPLICATIONINSIGHTS_CONNECTION_STRING");
            });
        }
    });

hostBuilder.Services.AddMemoryCache();

GatewayIntents gatewayIntents = GatewayIntents.AllUnprivileged - GatewayIntents.GuildInvites - GatewayIntents.GuildScheduledEvents;

#if DEBUG
DiscordSocketConfig discordSocketConfig = new()
{
    GatewayIntents = gatewayIntents,
    UseInteractionSnowflakeDate = false,
    LogLevel = LogSeverity.Debug
};
#else
DiscordSocketConfig discordSocketConfig = new()
{
    GatewayIntents = gatewayIntents
};
#endif

hostBuilder.Services
    .AddSingleton<DiscordSocketClient>(
        implementationInstance: new(discordSocketConfig)
    )
    .AddSingleton<IDiscordService, DiscordService>();

using var host = hostBuilder.Build();

var discordService = host.Services.GetRequiredService<IDiscordService>();
await discordService.ConnectAsync();

await host.RunAsync();