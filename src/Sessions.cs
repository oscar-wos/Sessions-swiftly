using Microsoft.Extensions.DependencyInjection;
using Sessions.API.Contracts.Core;
using Sessions.API.Contracts.Log;
using Sessions.Extensions;
using Sessions.Services.Core;
using Sessions.Services.Log;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Plugins;

namespace Sessions;

[PluginMetadata(
    Id = "sessions",
    Version = "0.0.0",
    Name = "Sessions",
    Website = "https://github.com/oscar-wos/Sessions-swiftly",
    Author = "oscar-wos"
)]
public partial class Sessions(ISwiftlyCore core) : BasePlugin(core)
{
    private IServiceProvider? _serviceProvider;
    private ILogService? _logService;

    public override void ConfigureSharedInterface(IInterfaceManager interfaceManager)
    {
        ServiceCollection services = new();

        _ = services.AddSwiftly(Core);
        _ = services.AddDatabase();

        _ = services.AddSingleton<IPlayerService, PlayerService>();
        _ = services.AddSingleton<IServerService, ServerService>();
        _ = services.AddSingleton<ILogService, LogService>();

        _serviceProvider = services.BuildServiceProvider();

        _logService = _serviceProvider.GetRequiredService<ILogService>();
        _logService.LogInformation("Loaded", logger: Core.Logger);
    }

    public override void Load(bool hotReload) { }

    public override void Unload() => (_serviceProvider as IDisposable)?.Dispose();
}
