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
using RSession.Shared.Contracts.Core;
using RSession.Shared.Contracts.Database;
using RSession.Shared.Structs;
using SwiftlyS2.Shared.Players;

namespace RSession.Contracts.Core;

internal interface IEventService : ISessionEventService
{
    void InvokeDatabaseConfigured(ISessionDatabaseService databaseService, string type);
    void InvokeElapsed(int interval);
    void InvokePlayerRegistered(IPlayer player, in SessionPlayer sessionPlayer);
    void InvokeServerRegistered(short serverId);
}
