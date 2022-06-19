using System;
using System.Collections.Generic;

namespace ChatAppServer.ClientConnections
{
	public interface IClientConnectionsCache
	{
		bool TryGetClientConnection(string connectionId, out ClientConnection clientConnection);
		
		void SaveClientConnection(string connectionId, ClientConnection clientConnection);

		void RemoveClientConnection(string connectionId);
		
		IEnumerable<string> GetActiveConnectionIds(IEnumerable<Guid> userIds);
	}
}