
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GameManager.Services
{
    public class ApplicationHostService : IHostedService
    {
        private readonly IHostApplicationLifetime _hostApplication;
        private readonly IGitHubService _gitHub;
        private readonly DataContext _dataContext;
        private readonly IInteropService _interop;
        private readonly ILibraryService _library;
        private readonly ISettingsService _settings;
        private readonly ILoaderService _loader;

        public ApplicationHostService(
            IHostApplicationLifetime hostApplication,
            IGitHubService gitHub,
            DataContext dataContext,
            IInteropService interop,
            ILibraryService library,
            ISettingsService settings,
            ILoaderService loader)
        {
            _hostApplication = hostApplication;
            _gitHub = gitHub;
            _dataContext = dataContext;
            _interop = interop;
            _library = library;
            _settings = settings;
            _loader = loader;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            UIElements.PageTitle("Loading . . .");

            UIElements.Normal("Checking Database");
            if ( _dataContext.Database.GetPendingMigrations().Any() )
            {
                UIElements.Warning("Updating Schema");
                _dataContext.Database.GetService<IMigrator>().Migrate();
            }

            UIElements.Success("OK!");
            UIElements.Blank();

            UIElements.Normal("Verifying Application Settings");
            if ( !_settings.HasF95Credentials() )
            {
                UIElements.Error($"F95 credentials are not set in the settings file. Update the settings file ({_settings.SettingsFilePath}) and restart the application.");
                _ = UIElements.TextInput("Press ENTER to close application");
                Environment.Exit(1);
            }

            UIElements.Success("OK!");
            UIElements.Blank();

            UIElements.Normal("Checking for Application Version");
            if ( await _gitHub.CheckIfNewerVersionExists() )
                UIElements.Success("A newer version is available!");
            else
                UIElements.Success("On latest version");
            UIElements.Blank();

            UIElements.Normal("Fetching Game Metadata from F95");
            try
            {
                await _loader.LoadMetadata();
                UIElements.Success("OK!");
            }
            catch ( Exception e )
            {
                UIElements.Error($"Cannot access F95: {e.Message}");
            }


            UIElements.Blank();

            UIElements.Normal("Checking library for updates");
            _library.CheckLibraryForUpdates();

            UIElements.Success("OK!");
            UIElements.Blank();

            UIElements.Divider();

            Console.Clear();

            var ui = new UI(_interop);
            ui.Start();

            _hostApplication.StopApplication();
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
