// Copyright (C) 2025 oscar-wos
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.
using Microsoft.Extensions.DependencyInjection;
using RSession.Rotation.Contracts.Core;
using RSession.Rotation.Extensions;
using RSession.Shared.Contracts.Core;
using RSession.Shared.Contracts.Event;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Plugins;

namespace RSession.Rotation;

[PluginMetadata(
    Id = "RSession.Rotation",
    Version = "1.1.1",
    Name = "RSession.Rotation",
    Website = "https://github.com/oscar-wos/RSession",
    Author = "oscar-wos"
)]
public sealed class Rotation(ISwiftlyCore core) : BasePlugin(core)
{
    private IServiceProvider? _serviceProvider;
    private ISessionEventService? _sessionEventService;

    public override void UseSharedInterface(IInterfaceManager interfaceManager)
    {
        if (interfaceManager.HasSharedInterface("RSession.EventService"))
        {
            _sessionEventService = interfaceManager.GetSharedInterface<ISessionEventService>(
                "RSession.EventService"
            );

            foreach (
                ISessionEventListener sessionEventListener in _serviceProvider?.GetServices<ISessionEventListener>()
                    ?? []
            )
            {
                sessionEventListener.Initialize(_sessionEventService);
            }
        }

        if (interfaceManager.HasSharedInterface("RSession.ServerService"))
        {
            ISessionServerService sessionServerService =
                interfaceManager.GetSharedInterface<ISessionServerService>(
                    "RSession.ServerService"
                );

            _serviceProvider?.GetRequiredService<IMapService>().Initialize(sessionServerService);
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

    public override void Unload()
    {
        _sessionEventService?.InvokeDispose();
        (_serviceProvider as IDisposable)?.Dispose();
    }
}
