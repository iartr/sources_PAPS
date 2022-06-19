using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatAppServer.Auth;
using ChatAppServer.ClientConnections;
using ChatAppServer.Dto;
using ChatAppServer.Exceptions;
using ChatAppServer.Models;
using ChatAppServer.Services;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace ChatAppServer.ClientHubs
{
	[EnableCors]
	public class ClientHub : Hub<IChatClient>
	{
		private readonly IClientConnectionsCache _clientConnectionsCache;
		private readonly IUsersService _usersService;
		private readonly IChatsService _chatsService;
		private readonly IMessagesService _messagesService;

		public ClientHub(IClientConnectionsCache clientConnectionsCache,
			IUsersService usersService, IChatsService chatsService,
			IMessagesService messagesService)
		{
			_clientConnectionsCache = clientConnectionsCache;
			_usersService = usersService;
			_chatsService = chatsService;
			_messagesService = messagesService;
		}

		public async Task SendPersonalMessageAsync(string receiverEmail, MessageDto messageDto)
		{
			Console.WriteLine($"SendPersonalMessageAsync {Context.ConnectionId}, {JsonConvert.SerializeObject(messageDto)}");
			if (_clientConnectionsCache.TryGetClientConnection(Context.ConnectionId, out var clientConnection))
			{
				var userId = clientConnection.User.Id;
				try
				{
					var saveMessageResult = await _messagesService.SavePersonalMessageAsync(userId, 
						receiverEmail, messageDto);
					
					if (saveMessageResult != null)
					{
						var (chatId, message, userIdsToNotify) = saveMessageResult;
						var connectionIds = _clientConnectionsCache.GetActiveConnectionIds(userIdsToNotify);
						await Clients.Clients(connectionIds).NotifyNewMessagePostedAsync(chatId, message);
					}
				}
				catch (UserNotFoundException e)
				{
					await Clients.Client(Context.ConnectionId).NotifyMessageReceivedAsync($"User not found: {e.Email}");
				}
			}
		}

		public async Task SendMessageToChatAsync(Guid chatId, MessageDto messageDto)
		{
			Console.WriteLine($"SendMessageToChatAsync {Context.ConnectionId}, {JsonConvert.SerializeObject(messageDto)}");
			if (_clientConnectionsCache.TryGetClientConnection(Context.ConnectionId, out var clientConnection))
			{
				var userId = clientConnection.User.Id;
				try
				{
					var saveMessageResult = await _messagesService.SaveMessageToChatAsync(userId, chatId, messageDto);
					if (saveMessageResult != null)
					{
						var (_, message, userIdsToNotify) = saveMessageResult;
						var connectionIds = _clientConnectionsCache.GetActiveConnectionIds(userIdsToNotify);
						await Clients.Clients(connectionIds).NotifyNewMessagePostedAsync(chatId, message);
					}
				}
				catch (UserNotFoundException e)
				{
					await Clients.Client(Context.ConnectionId).NotifyMessageReceivedAsync($"User not found: {e.Email}");
				}
			}
		}

		public async Task<User> GetUserByEmailAsync(string email)
		{
			Console.WriteLine($"GetUserByEmailAsync, email={email}");
			var user = await _usersService.GetUserByEmail(email);
			return user;
		}
		
		public async Task<List<ChatroomDto>> GetChatsAsync()
		{
			Console.WriteLine($"GetChatsAsync, {Context.ConnectionId}");
			if (!_clientConnectionsCache.TryGetClientConnection(Context.ConnectionId, out var clientConnection))
				return new List<ChatroomDto>();

			var chatsDtos = await _chatsService.GetAvailableChatsAsync(clientConnection.User.Id);
			return chatsDtos;
		}

		public async Task<List<MessageDto>> GetMessagesAsync(Guid chatId)
		{
			Console.WriteLine($"GetMessagesAsync, chatId = {chatId}");
			var messages = await _messagesService.GetChatMessagesAsync(chatId);
			return messages;
		}

		public async Task CreateChatroomAsync(NewChatroomDto newChatroomDto)
		{
			var connectionId = Context.ConnectionId;
			Console.WriteLine($"CreateChatroomAsync, chatroomDto = {JsonConvert.SerializeObject(newChatroomDto)}");
			
			try
			{
				if (!_clientConnectionsCache.TryGetClientConnection(connectionId, out var clientConnection))
				{
					await Clients.Client(connectionId).NotifyMessageReceivedAsync("Did not find client connection");
					return;
				}

				await _chatsService.CreateChatroomAsync(clientConnection.User.Id, newChatroomDto);
				await Clients.Client(connectionId).NotifyMessageReceivedAsync("Successfully created chatroom!");
			}
			catch (Exception e)
			{
				Console.WriteLine(e);	
				await Clients.Client(connectionId).NotifyMessageReceivedAsync("Internal server error while creating chatroom");
			}
		}
		public async Task UpdateChatroomAsync(NewChatroomDto updatedChatroomDto)
		{
			var connectionId = Context.ConnectionId;
			Console.WriteLine($"UpdateChatroomAsync, {nameof(updatedChatroomDto)} = {JsonConvert.SerializeObject(updatedChatroomDto)}");
			
			try
			{
				if (!_clientConnectionsCache.TryGetClientConnection(connectionId, out var clientConnection))
				{
					await Clients.Client(connectionId).NotifyMessageReceivedAsync("Did not find client connection");
					return;
				}

				await _chatsService.UpdateChatroomAsync(updatedChatroomDto);
				await Clients.Client(connectionId).NotifyMessageReceivedAsync("Successfully created chatroom!");
			}
			catch (Exception e)
			{
				Console.WriteLine(e);	
				await Clients.Client(connectionId).NotifyMessageReceivedAsync("Internal server error while creating chatroom");
			}
		}
		
		
		
		public override async Task OnConnectedAsync()
		{
			var httpContext = Context.GetHttpContext();
			var accessTokenBearer = httpContext.Request.Query["access_token"].ToString();
			var connectionId = Context.ConnectionId;
			
			try
			{
				if (TryExtractAccessToken(accessTokenBearer, out var accessToken))
				{
					var payload = await GoogleJsonWebSignature.ValidateAsync(accessToken);
					var googleUserInfo = ConvertPayloadToGoogleUserInfo(payload);

					var user = await _usersService.AuthGoogleUserAsync(googleUserInfo);
					Console.WriteLine($"Authenticated user: {JsonConvert.SerializeObject(user)}");
					
					_clientConnectionsCache.SaveClientConnection(connectionId, new ClientConnection(accessToken, user));
				}

				Console.WriteLine($"Connected: {connectionId}");

				await base.OnConnectedAsync();
			}
			catch (InvalidJwtException invalidJwtException)
			{
				await Clients.Client(connectionId).NotifyMessageReceivedAsync("Invalid jwt");
				Console.WriteLine(invalidJwtException.Message + $" current time: {DateTime.UtcNow}");
				Context.Abort();
			}
			catch (Exception exception)
			{
				var exceptionJson = JsonConvert.SerializeObject(exception);
				Console.WriteLine(exceptionJson);
				await Clients.Client(connectionId).NotifyMessageReceivedAsync("User not found");
				await base.OnConnectedAsync();
			}
		}

		public override Task OnDisconnectedAsync(Exception exception)
		{
			Console.WriteLine($"Disconnected: {Context.ConnectionId}");
			return base.OnDisconnectedAsync(exception);
		}

		private static bool TryExtractAccessToken(string accessTokenBearer, out string accessToken)
		{
			accessToken = string.Empty;
			var accessTokenBearerParts = accessTokenBearer.Split();
			if (accessTokenBearerParts.Length != 2)
				return false;

			if (accessTokenBearerParts[0] != "Bearer")
				return false;

			accessToken = accessTokenBearerParts[1];
			return true;
		}

		private static GoogleUserInfo ConvertPayloadToGoogleUserInfo(GoogleJsonWebSignature.Payload payload)
		{
			var googleUserInfo = new GoogleUserInfo
			{
				Name = payload.Name,
				Email = payload.Email,
				ProfilePictureUrl = payload.Picture,
			};

			return googleUserInfo;
		}
	}
}