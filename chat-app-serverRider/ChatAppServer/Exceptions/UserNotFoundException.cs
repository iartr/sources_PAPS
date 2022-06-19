using System;

namespace ChatAppServer.Exceptions
{
	public class UserNotFoundException : Exception
	{
		public UserNotFoundException(Guid userId, string message = "", Exception inner = null)
			: base(message, inner)
		{
			UserId = userId;
		}

		public UserNotFoundException(string email, string message = "", Exception inner = null) 
			: base(message, inner)
		{
			Email = email;
		}

		public string Email { get; }
		public Guid UserId { get; }
	}
}