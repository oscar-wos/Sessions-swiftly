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
using RSession.Shared.Contracts.Database;

namespace RSession.Contracts.Database;

internal interface IDatabaseService : ISessionDatabaseService
{
    Task CreateTablesAsync();
    Task<int> GetPlayerAsync(ulong steamId);
    Task<short> GetServerAsync(string serverIp, ushort serverPort);
    Task<long> GetSessionAsync(int playerId, short serverId, string ip);
    Task UpdateSessionsAsync(List<int> playerIds, List<long> sessionIds);
}
