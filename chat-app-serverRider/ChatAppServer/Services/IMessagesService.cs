using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatAppServer.Dto;
using ChatAppServer.Models;

namespace ChatAppServer.Services
{
	public interface IMessagesService
	{
		Task<SaveMessageResult> SaveMessageToChatAsync(Guid senderId, Guid chatId, MessageDto messageDto);

		Task<SaveMessageResult> SavePersonalMessageAsync(Guid senderId, string receiverEmail, MessageDto messageDto);

		Task<List<MessageDto>> GetChatMessagesAsync(Guid chatId);
	}
}