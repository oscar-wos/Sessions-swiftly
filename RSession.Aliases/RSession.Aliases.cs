// Copyright (C) 2025 oscar-wos
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.
using Microsoft.Extensions.DependencyInjection;
using RSession.Aliases.Contracts.Core;
using RSession.Aliases.Contracts.Event;
using RSession.Aliases.Extensions;
using RSession.Shared.Contracts;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Plugins;

namespace RSession.Aliases;

[PluginMetadata(
    Id = "RSession.Aliases",
    Version = "0.0.0",
    Name = "RSession.Aliases",
    Website = "https://github.com/oscar-wos/RSession",
    Author = "oscar-wos"
)]
public sealed partial class Aliases(ISwiftlyCore core) : BasePlugin(core)
{
    private IServiceProvider? _serviceProvider;

    public override void UseSharedInterface(IInterfaceManager interfaceManager)
    {
        if (interfaceManager.HasSharedInterface("RSession.EventService"))
        {
            ISessionEventService sessionEventService =
                interfaceManager.GetSharedInterface<ISessionEventService>("RSession.EventService");

            _serviceProvider
                ?.GetService<IOnDatabaseConfiguredService>()
                ?.Initialize(sessionEventService);
        }

        if (interfaceManager.HasSharedInterface("RSession.EventService"))
        {
            ISessionPlayerService sessionPlayerService =
                interfaceManager.GetSharedInterface<ISessionPlayerService>(
                    "RSession.PlayerService"
                );

            _serviceProvider?.GetService<IPlayerService>()?.Initialize(sessionPlayerService);
        }
    }

    public override void Load(bool hotReload)
    {
        ServiceCollection services = new();

        _ = services.AddSwiftly(Core);

        _ = services.AddDatabases();
        _ = services.AddEvents();
        _ = services.AddServices();

        _serviceProvider = services.BuildServiceProvider();
    }

    public override void Unload() => (_serviceProvider as IDisposable)?.Dispose();
}
