using System;
using System.Threading.Tasks;
using AutoMapper;
using ChatAppServer.Auth;
using ChatAppServer.DataAccess;
using ChatAppServer.DataAccess.Entities;
using ChatAppServer.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatAppServer.Services
{
	public class UsersService : IUsersService
	{
		private readonly ChatDbContext _chatDbContext;
		private readonly IMapper _mapper;

		public UsersService(ChatDbContext chatDbContext,
			IMapper mapper)
		{
			_chatDbContext = chatDbContext;
			_mapper = mapper;
		}
		
		public async Task<User> AuthGoogleUserAsync(GoogleUserInfo googleUserInfo)
		{
			var user = _mapper.Map<User>(googleUserInfo);
			
			try
			{
				var existingUserEntity =
					await _chatDbContext.Users.FirstOrDefaultAsync(userEntity => userEntity.Email == user.Email);
			
				if (existingUserEntity != null)
				{
					await UpdateUserIfNeedAsync(existingUserEntity, user);
					user = _mapper.Map<User>(existingUserEntity);
				}
				else
				{
					var newUserEntity = _mapper.Map<UserEntity>(user);
					await _chatDbContext.Users.AddAsync(newUserEntity);
					await _chatDbContext.SaveChangesAsync();
					user = _mapper.Map<User>(newUserEntity);
				}

				return user;
			}
			catch (Exception e)
			{
				Console.WriteLine($"Exception: {e}");
				throw;
			}
		}

		public async Task<User> GetUserByEmail(string email)
		{
			var userEntity = await _chatDbContext.Users.FirstOrDefaultAsync(entity => entity.Email == email);

			if (userEntity == null) 
				return null;
			
			var user = _mapper.Map<User>(userEntity);

			return user;
		}

		private async Task UpdateUserIfNeedAsync(UserEntity existingUserEntity, User user)
		{
			var fieldsChanged = existingUserEntity.Name != user.Name ||
			                      existingUserEntity.ProfilePictureUrl != user.ProfilePictureUrl;
			if (fieldsChanged)
			{
				existingUserEntity.Name = user.Name;
				existingUserEntity.ProfilePictureUrl = user.ProfilePictureUrl;
				await _chatDbContext.SaveChangesAsync();
			}
		}
	}
}