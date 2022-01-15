
using Microsoft.Extensions.DependencyInjection;

Console.Title = UIElements.ApplicationTitle;

await Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddSingleton<InteropService>();
        services.AddHostedService<ApplicationHostService>();
    })
    .RunConsoleAsync();
