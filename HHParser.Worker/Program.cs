using HHParser.Worker;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService()
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
