using System;
using System.Collections.Generic;

namespace ChatAppServer.Dto
{
	public class NewChatroomDto
	{
		public Guid? Id { get; set; }
		
		public string Name { get; set; }

		public IEnumerable<Guid> MembersIds { get; set; }
	}
}