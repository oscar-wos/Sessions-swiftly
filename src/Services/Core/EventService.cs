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
using RSession.Contracts.Core;
using RSession.Shared.Contracts.Database;
using RSession.Shared.Delegates;
using RSession.Shared.Structs;
using SwiftlyS2.Shared.Players;

namespace RSession.Services.Core;

internal sealed class EventService : IEventService
{
    public event OnDatabaseConfiguredDelegate? OnDatabaseConfigured;
    public event OnDisposeDelegate? OnDispose;
    public event OnElapsedDelegate? OnElapsed;
    public event OnPlayerRegisteredDelegate? OnPlayerRegistered;
    public event OnServerRegisteredDelegate? OnServerRegistered;

    public void InvokeDatabaseConfigured(
        ISessionDatabaseService sessionDatabaseService,
        string type,
        string prefix
    ) => OnDatabaseConfigured?.Invoke(sessionDatabaseService, type, prefix);

    public void InvokeDispose() => OnDispose?.Invoke();

    public void InvokeElapsed(int interval) => OnElapsed?.Invoke(interval);

    public void InvokePlayerRegistered(IPlayer player, in SessionPlayer sessionPlayer) =>
        OnPlayerRegistered?.Invoke(player, in sessionPlayer);

    public void InvokeServerRegistered(short serverId) => OnServerRegistered?.Invoke(serverId);
}
