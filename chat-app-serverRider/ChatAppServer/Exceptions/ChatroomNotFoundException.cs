using System;

namespace ChatAppServer.Exceptions
{
	public class ChatroomNotFoundException : Exception
	{
		public ChatroomNotFoundException(Guid chatId, string message = "", Exception inner = null)
			: base(message, inner)
		{
			ChatId = chatId;
		}

		public Guid ChatId { get; set; }
	}
}