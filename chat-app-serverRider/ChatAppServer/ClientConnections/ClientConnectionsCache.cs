using System;
using System.Collections.Generic;

namespace ChatAppServer.ClientConnections
{
	public class ClientConnectionsCache : IClientConnectionsCache
	{
		private readonly Dictionary<string, ClientConnection> _clientIdToClientConnectionMap = new();

		private readonly Dictionary<Guid, HashSet<string>> _userIdToConnectionIdMap = new();

		public bool TryGetClientConnection(string connectionId, out ClientConnection clientConnection)
		{
			var clientConnectionFound = _clientIdToClientConnectionMap.TryGetValue(connectionId, out clientConnection);
			return clientConnectionFound;
		}

		public void SaveClientConnection(string connectionId, ClientConnection clientConnection)
		{
			if (connectionId == null) throw new ArgumentNullException(nameof(connectionId));
			if (clientConnection == null) throw new ArgumentNullException(nameof(clientConnection));
			
			_clientIdToClientConnectionMap[connectionId] = clientConnection;
			if (!_userIdToConnectionIdMap.ContainsKey(clientConnection.User.Id))
				_userIdToConnectionIdMap[clientConnection.User.Id] = new HashSet<string>();
			
			_userIdToConnectionIdMap[clientConnection.User.Id].Add(connectionId);
		}

		public void RemoveClientConnection(string connectionId)
		{
			_clientIdToClientConnectionMap.Remove(connectionId);
		}

		public IEnumerable<string> GetActiveConnectionIds(IEnumerable<Guid> userIds)
		{
			var connectionIds = new List<string>();
			foreach (var userId in userIds)
			{
				if (_userIdToConnectionIdMap.TryGetValue(userId, out var userConnectionIds))
					connectionIds.AddRange(userConnectionIds);
			}

			return connectionIds;
		}
	}
}