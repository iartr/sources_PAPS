using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatAppServer.Dto;

namespace ChatAppServer.Services
{
	public interface IChatsService
	{
		Task<List<ChatroomDto>> GetAvailableChatsAsync(Guid userId);

		Task<bool> CreateChatroomAsync(Guid creatorId, NewChatroomDto newChatroomDto);

		Task<bool> UpdateChatroomAsync(NewChatroomDto updatedChatroomDto);
	}
}