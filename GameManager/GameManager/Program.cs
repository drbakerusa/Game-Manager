
using Microsoft.Extensions.DependencyInjection;

Console.Title = UIElements.ApplicationTitle;

await Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddDbContext<DataContext>();

        services.AddSingleton<ILibraryService, LibraryService>();
        services.AddSingleton<IGitHubService, GitHubService>();
        services.AddSingleton<IInteropService, InteropService>();
        services.AddSingleton<ILoaderService, LoaderService>();
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddHostedService<ApplicationHostService>();
    })
    .RunConsoleAsync();
