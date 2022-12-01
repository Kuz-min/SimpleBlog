using Microsoft.Extensions.DependencyInjection;

namespace SimpleBlog.StartupTasks
{
    public class StartupTasksRunner : IHostedService
    {
        public StartupTasksRunner(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await new AuthenticationStartupTask(_serviceProvider).Run();


        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private readonly IServiceProvider _serviceProvider;
    }
}
