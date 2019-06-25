using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedLibrary.Enums;

namespace SharedLibrary.Interfaces
{
	public interface ITicket : IDbEntity, IMessage
	{
		int? AdminId { get; set; }
		int CategoryId { get; set; }
		string Title { get; set; }
		TicketStatus Status { get; set; }
		DateTime Created { get; set; }
		DateTime LastActivity { get; }

		string Name { get; }
		string Email { get; }
	}

	public interface ITicket<T> : ITicket where T : ITicketComment
	{
		ICollection<T> Comments { get; set; }
	}
}