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
using RSession.Played.Contracts.Event;
using RSession.Played.Extensions;
using RSession.Shared.Contracts;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Plugins;

namespace RSession.Played;

[PluginMetadata(
    Id = "RSession.Played",
    Version = "0.0.0",
    Name = "RSession.Played",
    Website = "https://github.com/oscar-wos/RSession",
    Author = "oscar-wos"
)]
public sealed partial class Played(ISwiftlyCore core) : BasePlugin(core)
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
                IEventListener eventListener in _serviceProvider?.GetServices<IEventListener>()
                    ?? []
            )
            {
                eventListener.Initialize(_sessionEventService);
            }
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
