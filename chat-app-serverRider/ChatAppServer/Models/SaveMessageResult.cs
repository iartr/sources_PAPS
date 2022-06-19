using System;
using System.Collections.Generic;
using ChatAppServer.Dto;

namespace ChatAppServer.Models
{
	public class SaveMessageResult
	{
		public Guid ChatId { get; set; }

		public MessageDto Message { get; set; }

		public IEnumerable<Guid> UserIdsToNotify { get; set; }

		public void Deconstruct(out Guid chatId, out MessageDto message, out IEnumerable<Guid> userIdsToNotify)
		{
			chatId = ChatId;
			message = Message;
			userIdsToNotify = UserIdsToNotify;
		}
	}
}