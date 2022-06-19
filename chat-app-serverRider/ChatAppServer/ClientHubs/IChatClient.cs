using System;
using System.Threading.Tasks;
using ChatAppServer.Dto;

namespace ChatAppServer.ClientHubs
{
	public interface IChatClient
	{
		Task NotifyMessageReceivedAsync(string message);

		Task NotifyNewMessagePostedAsync(Guid chatId, MessageDto messageDto);
	}
}