using Microsoft.Extensions.DependencyInjection;
using Sessions.API.Contracts.Core;
using Sessions.API.Contracts.Log;
using Sessions.API.Models;
using Sessions.Extensions;
using Sessions.Services.Core;
using Sessions.Services.Log;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Plugins;
using Tomlyn.Extensions.Configuration;

namespace Sessions;

[PluginMetadata(
    Id = "Sessions",
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

        _logService.LogInformation("Loading config", logger: Core.Logger);

        _ = Core
            .Configuration.InitializeTomlWithModel<DatabaseConfig>("database.toml", "database")
            .Configure(builder =>
                builder.AddTomlFile("database.toml", optional: false, reloadOnChange: true)
            );

        _ = services.AddOptionsWithValidateOnStart<DatabaseConfig>().BindConfiguration("database");

        _logService.LogInformation("Loaded", logger: Core.Logger);
    }

    public override void UseSharedInterface(IInterfaceManager interfaceManager) { }

    public override void Load(bool hotReload) { }

    public override void Unload() => (_serviceProvider as IDisposable)?.Dispose();
}
