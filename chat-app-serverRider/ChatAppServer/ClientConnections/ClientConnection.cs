using ChatAppServer.Models;

namespace ChatAppServer.ClientConnections
{
	public class ClientConnection
	{
		public ClientConnection(string accessToken, User user)
		{
			AccessToken = accessToken;
			User = user;
		}
		
		public string AccessToken { get;}

		public User User { get; set; }
	}
}