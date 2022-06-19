using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatAppServer.DataAccess.Entities
{
	[Table("Chatroom")]
	public class ChatroomEntity
	{
		public Guid Id { get; set; }

		public string Name { get; set; }
		
		public Guid? OwnerId { get; set; }

		public ICollection<ChatroomMessageEntity> Messages { get; set; }
		public ICollection<UserEntity> Members { get; set; }
		
		public UserEntity Owner { get; set; }
	}
}