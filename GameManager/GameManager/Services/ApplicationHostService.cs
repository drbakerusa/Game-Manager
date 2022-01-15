namespace GameManager.Services
{
    public class ApplicationHostService : IHostedService
    {
        private readonly IHostApplicationLifetime _hostApplication;

        public ApplicationHostService(IHostApplicationLifetime hostApplication)
        {
            _hostApplication = hostApplication;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var ui = new UI();
            ui.Start();

            _hostApplication.StopApplication();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
