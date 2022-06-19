using System.Threading.Tasks;
using ChatAppServer.Auth;
using ChatAppServer.Models;

namespace ChatAppServer.Services
{
	public interface IUsersService
	{
		Task<User> AuthGoogleUserAsync(GoogleUserInfo googleUserInfo);

		Task<User> GetUserByEmail(string email);
	}
}