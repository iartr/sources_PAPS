using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ChatAppServer.DataAccess;
using ChatAppServer.DataAccess.Entities;
using ChatAppServer.Dto;
using ChatAppServer.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ChatAppServer.Services
{
	public class ChatsService : IChatsService
	{
		private readonly ChatDbContext _chatDbContext;
		private readonly IMapper _mapper;

		public ChatsService(ChatDbContext chatDbContext,
			IMapper mapper)
		{
			_chatDbContext = chatDbContext;
			_mapper = mapper;
		}
		
		public async Task<List<ChatroomDto>> GetAvailableChatsAsync(Guid userId)
		{
			var user = await _chatDbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
			
			if (user == null)
				throw new UserNotFoundException(userId);
			
			var chatroomMiddleDtos = await _chatDbContext.Chatrooms
				                   .Include(chatroom => chatroom.Messages)
				                   .Include(chatroom => chatroom.Members)
				                   .Where(chatroom => chatroom.Members.Contains(user))
				                   .Select(chatroom => new AlmostChatroomDto
				                   {
					                   Id = chatroom.Id,
					                   Owner = chatroom.Owner,
					                   Name = chatroom.Name,
					                   Members = chatroom.Members,
					                   LastMessage = chatroom.Messages.OrderByDescending(m => m.SentTime).FirstOrDefault(),
				                   })
				                   .ToListAsync();

			var chatroomDtos = chatroomMiddleDtos.Select(dto => ConvertToChatroomDto(dto, userId)).ToList();

			return chatroomDtos;
		}

		public async Task<bool> CreateChatroomAsync(Guid creatorId, NewChatroomDto newChatroomDto)
		{
			var membersIdsSet = new HashSet<Guid>(newChatroomDto.MembersIds);
			var members = await _chatDbContext.Users
				              .Where(user => membersIdsSet.Contains(user.Id))
				              .ToListAsync();

			var owner = await _chatDbContext.Users.FindAsync(creatorId);
			if (owner == null)
				throw new UserNotFoundException(creatorId, "Creator of chatroom was not found");

			if (members.All(member => member.Id != creatorId))
				members.Add(owner);

			var chatroom = new ChatroomEntity
			{
				Name = newChatroomDto.Name,
				Members = members,
				Owner = owner,
			};

			await _chatDbContext.Chatrooms.AddAsync(chatroom);
			await _chatDbContext.SaveChangesAsync();

			return true;
		}

		public async Task<bool> UpdateChatroomAsync(NewChatroomDto updatedChatroomDto)
		{
			var chatroomId = updatedChatroomDto.Id 
			                 ?? throw new ArgumentNullException(nameof(updatedChatroomDto), 
				                 $"Id of {nameof(updatedChatroomDto)} was null");

			var chatroom = await _chatDbContext.Chatrooms
				               .Include(chat => chat.Members)
				               .FirstOrDefaultAsync(chat => chat.Id == chatroomId);
			
			if (chatroom == null)
				throw new ChatroomNotFoundException(chatroomId, "Unable to update chatroom");

			var membersIdsSet = new HashSet<Guid>(updatedChatroomDto.MembersIds);
			var members = await _chatDbContext.Users
				              .Where(user => membersIdsSet.Contains(user.Id))
				              .ToListAsync();
			
			chatroom.Name = updatedChatroomDto.Name;
			chatroom.Members = members;
			
			await _chatDbContext.SaveChangesAsync();

			return true;
		}

		private ChatroomDto ConvertToChatroomDto(AlmostChatroomDto almostChatroomDto, Guid currentUserId)
		{
			ChatType chatType;
			if (almostChatroomDto.Owner == null && almostChatroomDto.Members.Count == 2)
				chatType = ChatType.PersonalChat;
			else
				chatType = ChatType.Chatroom;
			
			var chatroomDto = new ChatroomDto
			{
				Id = almostChatroomDto.Id,
				ChatType = chatType,
				Name = almostChatroomDto.Name,
				Owner = almostChatroomDto.Owner == null ? null : _mapper.Map<UserDto>(almostChatroomDto.Owner),
				Members = almostChatroomDto.Members.Select(_mapper.Map<UserDto>),
				LastMessage = almostChatroomDto.LastMessage?.Text,
				LastMessageTime = almostChatroomDto.LastMessage != null
					                  ? DateTime.SpecifyKind(almostChatroomDto.LastMessage.SentTime, DateTimeKind.Utc)
					                  : null,
			};
			
			if (chatType == ChatType.PersonalChat)
			{
				var otherMember = almostChatroomDto.Members.First(member => member.Id != currentUserId);
				chatroomDto.ImageUrl = otherMember.ProfilePictureUrl;
				chatroomDto.Name = otherMember.Name;
			}

			return chatroomDto;
		}

		private class AlmostChatroomDto
		{
			public Guid Id { get; set; }
			public UserEntity Owner { get; set; }
			public string Name { get; set; }
			public ICollection<UserEntity> Members { get; set; }
			public ChatroomMessageEntity? LastMessage { get; set; }
		}
	}
}