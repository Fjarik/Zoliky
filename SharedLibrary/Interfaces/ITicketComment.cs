using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.Interfaces
{
	public interface ITicketComment : IMessage
	{
		int TicketId { get; set; }
		DateTime Created { get; set; }
		string Name { get; }
		bool IsFromAdmin { get; }
	}
}
