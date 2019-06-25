using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using Microsoft.Owin;
using SharedLibrary;
using SharedLibrary.Enums;
using SharedLibrary.Interfaces;

namespace DataAccess.Managers.Support
{
	public abstract class UniversalTicketManager<T> : Manager<T> where T : class, IDbEntity, ITicket
	{
		/// 
		/// Constructors
		/// 
		protected UniversalTicketManager(IOwinContext context) : this(context, new ZoliksEntities()) { }

		protected UniversalTicketManager(IOwinContext context, ZoliksEntities ctx) : base(context, ctx) { }

		// Methods

#region Methods

		/// 
		/// Own methods
		///

#region Own Methods

		public virtual async Task<MActionResult<List<T>>> GetByAdminIdAsync(int adminId)
		{
			if (adminId < 1) {
				return new MActionResult<List<T>>(StatusCode.NotValidID);
			}

			var list = await _ctx.Set<T>()
								 .Where(x => x.AdminId == adminId)
								 .OrderByDescending(x => x.Created)
								 .ToListAsync();
			return new MActionResult<List<T>>(StatusCode.OK, list);
		}

		// Všechny tickety, které nebyly přiřazeny podpoře
		public virtual async Task<MActionResult<List<T>>> GetAllUnallocatedAsync()
		{
			var list = await _ctx.Set<T>()
								 .Where(x => x.AdminId == null)
								 .OrderByDescending(x => x.Created)
								 .ToListAsync();
			return new MActionResult<List<T>>(StatusCode.OK, list);
		}

		public virtual async Task<MActionResult<T>> AssignAsync(int ticketId, int adminId)
		{
			if (ticketId < 1 || adminId < 1) {
				return new MActionResult<T>(StatusCode.NotValidID);
			}
			var tRes = await this.GetByIdAsync(ticketId);
			if (!tRes.IsSuccess) {
				return tRes;
			}
			var ticket = tRes.Content;
			ticket.AdminId = adminId;
			ticket.Status = TicketStatus.Open;
			await this.SaveAsync(ticket);

			return new MActionResult<T>(StatusCode.OK, ticket);
		}

		public virtual async Task<MActionResult<T>> ChangeStatusAsync(int ticketId, TicketStatus status)
		{
			if (ticketId < 1) {
				return new MActionResult<T>(StatusCode.NotValidID);
			}
			var tRes = await this.GetByIdAsync(ticketId);
			if (!tRes.IsSuccess) {
				return tRes;
			}
			var ticket = tRes.Content;
			ticket.Status = status;
			await this.SaveAsync(ticket);

			return new MActionResult<T>(StatusCode.OK, ticket);
		}

		public virtual Task<MActionResult<T>> CloseAsync(int ticketId)
		{
			return this.ChangeStatusAsync(ticketId, TicketStatus.Closed);
		}

#endregion

#endregion
	}
}