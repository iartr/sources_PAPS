using ChatAppServer.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatAppServer.DataAccess
{
	public class ChatDbContext : DbContext
	{
		public ChatDbContext(DbContextOptions options) : base(options)
		{
		}
		
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<ChatroomEntity>()
				.HasMany(chatroom => chatroom.Members)
				.WithMany(member => member.Chatrooms)
				.UsingEntity(join => join.ToTable("ChatroomMember"));

			modelBuilder.Entity<ChatroomEntity>()
				.HasOne(chatroom => chatroom.Owner)
				.WithMany(user => user.OwnedChatrooms)
				.HasForeignKey(chatroom => chatroom.OwnerId);

			modelBuilder.Entity<ChatroomEntity>()
				.HasMany(chatroom => chatroom.Messages)
				.WithOne(message => message.Chatroom)
				.OnDelete(DeleteBehavior.Cascade);
		}

		public DbSet<UserEntity> Users { get; set; }
		public DbSet<ChatroomEntity> Chatrooms { get; set; }
		public DbSet<ChatroomMessageEntity> ChatroomMessages { get; set; }
	}
}