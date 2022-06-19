using System;
using System.Collections.Generic;

namespace ChatAppServer.Dto
{
	public class ChatroomDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public ChatType ChatType { get; set; }
		public UserDto Owner { get; set; }
		public IEnumerable<UserDto> Members { get; set; }
		public DateTime? LastMessageTime { get; set; }
		public string LastMessage { get; set; }
		public string ImageUrl { get; set; }
	}

	public enum ChatType
	{
		PersonalChat = 1,
		Chatroom = 2,
	}
}