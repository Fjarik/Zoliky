using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using JetBrains.Annotations;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using SharedLibrary;
using SharedLibrary.Enums;

namespace DataAccess.Managers.Support
{
	public class TicketManager : UniversalTicketManager<Ticket>
	{
		/// 
		/// Fields
		///
		/// 
		/// Constructors
		/// 
		public TicketManager(IOwinContext context) : this(context, new ZoliksEntities()) { }

		public TicketManager(IOwinContext context, ZoliksEntities ctx) : base(context, ctx) { }

		// Methods

#region Methods

		/// 
		/// Overrides
		///

#region Overrides

#endregion

		/// 
		/// Own methods
		/// 

#region Static methods

		public static TicketManager Create(IdentityFactoryOptions<TicketManager> options,
										   IOwinContext context)
		{
			return new TicketManager(context);
		}

#endregion

#region Own Methods

#endregion

#endregion
	}
}