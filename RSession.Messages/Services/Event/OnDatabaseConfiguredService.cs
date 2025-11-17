using Microsoft.Extensions.Logging;
using RSession.Messages.Contracts.Database;
using RSession.Messages.Contracts.Event;
using RSession.Messages.Contracts.Log;
using RSession.Shared.Contracts;

namespace RSession.Messages.Services.Event;

internal sealed class OnDatabaseConfiguredService(
    ILogService logService,
    ILogger<OnDatabaseConfiguredService> logger,
    IDatabaseFactory databaseFactory
) : IOnDatabaseConfiguredService, IDisposable
{
    private readonly ILogService _logService = logService;
    private readonly ILogger<OnDatabaseConfiguredService> _logger = logger;

    private readonly IDatabaseFactory _databaseFactory = databaseFactory;

    private ISessionEventService? _sessionEventService;

    public void Initialize(ISessionEventService sessionEventService)
    {
        _sessionEventService = sessionEventService;

        _sessionEventService.OnDatabaseConfigured += OnDatabaseConfigured;
        _logService.LogInformation("OnDatabaseConfigured subscribed", logger: _logger);
    }

    private void OnDatabaseConfigured(ISessionDatabaseService databaseService, string type) =>
        _databaseFactory.RegisterDatabaseService(databaseService, type);

    public void Dispose()
    {
        _sessionEventService?.OnDatabaseConfigured -= OnDatabaseConfigured;
        _logService.LogInformation("OnDatabaseConfiguredService disposed", logger: _logger);
    }
}
