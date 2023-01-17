namespace SimpleBlog.StartupTasks;

public class StartupTasksRunner : IHostedService
{
    public StartupTasksRunner(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await new DefaultClientApplicationsSetup(_serviceProvider).Run();
        await new DefaultRolesSetup(_serviceProvider).Run();
        await new DefaultAccountsSetup(_serviceProvider).Run();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private readonly IServiceProvider _serviceProvider;
}