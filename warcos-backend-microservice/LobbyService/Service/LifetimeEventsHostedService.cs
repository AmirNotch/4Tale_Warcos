namespace Lobby.Service;

internal class LifetimeEventsHostedService(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<LifetimeEventsHostedService> _logger,
    IHostApplicationLifetime _appLifetime) : IHostedService {
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _appLifetime.ApplicationStarted.Register(OnStarted);
        _appLifetime.ApplicationStopping.Register(OnStopping);
        _appLifetime.ApplicationStopped.Register(OnStopped);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private void OnStarted()
    {
    }

    private async void OnStopping()
    {
        _logger.LogInformation("OnStopping has been called.");

        using (IServiceScope scope = serviceScopeFactory.CreateScope())
        {
            WebSocketHandler userWebSocketService = scope.ServiceProvider.GetRequiredService<WebSocketHandler>();
            IEnumerable<Guid> userIds = WebSocketConnectionManager.GetConnectedUserIds();
            foreach (Guid userId in userIds)
            {
                await userWebSocketService.FinalizeWsConnection(userId);
            }
        }
    }

    private void OnStopped()
    {
    }
}
