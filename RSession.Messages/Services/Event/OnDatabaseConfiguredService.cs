using Microsoft.Extensions.Logging;
using RSession.Messages.Contracts.Database;
using RSession.Messages.Contracts.Log;
using RSession.Shared.Contracts;

namespace RSession.Messages.Services.Event;

internal sealed class OnDatabaseConfiguredService(
    ILogService logService,
    ILogger<OnDatabaseConfiguredService> logger,
    IDatabaseFactory databaseFactory
) : IDisposable
{
    private readonly ILogService _logService = logService;
    private readonly ILogger<OnDatabaseConfiguredService> _logger = logger;

    private readonly IDatabaseFactory _databaseFactory = databaseFactory;

    private ISessionEventService? _eventService;

    public void Initialize(ISessionEventService eventService)
    {
        _eventService = eventService;

        _eventService.OnDatabaseConfigured += OnDatabaseConfigured;
        _logService.LogInformation("OnDatabaseConfigured subscribed", logger: _logger);
    }

    private void OnDatabaseConfigured(ISessionDatabaseService databaseService, string type)
    {
        _databaseFactory.RegisterDatabaseService(databaseService, type);
    }

    public void Dispose()
    {
        _eventService?.OnDatabaseConfigured -= OnDatabaseConfigured;
        _logService.LogInformation("OnDatabaseConfiguredService disposed", logger: _logger);
    }
}
