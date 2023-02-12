using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TNArch.DistributedConcurrency.Core;
using TNArch.DistributedConcurrency.Core.Repository;
using TNArch.DistributedConcurrency.Demo;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        string storageConnectionString = "storage connections string here";
        services.AddScoped<IConcurrencyLockRepository>(sp => ActivatorUtilities.CreateInstance<ConcurrencyLockRepository>(sp, storageConnectionString));
        services.AddScoped<IConcurrencyLockService, ConcurrencyLockService>();
        services.AddHostedService<DemoHostedService>();
    })
    .Build();

await host.RunAsync();



