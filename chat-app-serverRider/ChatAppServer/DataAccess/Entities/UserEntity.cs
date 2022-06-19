using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ChatAppServer.DataAccess.Entities
{
	[Table("User")]
	[Index(nameof(Email), IsUnique = true)]
	public class UserEntity
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; set; }

		[Required]
		public string Name { get; set; }

		
		[Required]
		public string Email { get; set; }

		[Required]
		public string ProfilePictureUrl { get; set; }

		public ICollection<ChatroomEntity> Chatrooms { get; set; }

		public ICollection<ChatroomEntity> OwnedChatrooms { get; set; }
	}
}