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

namespace DataAccess.Managers
{
	public class ProjectManager : Manager<Project>
	{
		/// 
		/// Fields
		///
		/// 
		/// Constructors
		/// 
		public ProjectManager(IOwinContext context) : this(context, new ZoliksEntities()) { }

		public ProjectManager(IOwinContext context, ZoliksEntities ctx) : base(context, ctx) { }

		// Methods

#region Methods

#region Overrides

		public override Task<MActionResult<Project>> CreateAsync(Project entity)
		{
			throw new
				NotSupportedException("Vytváření projektů není možné! Zapište ručně do databáze a posléze implementujte");
		}

		public override Task<bool> DeleteAsync(Project entity)
		{
			throw new NotSupportedException("Mazání projektů není možné!");
		}

#endregion

		/// 
		/// Own methods
		/// 

#region Static methods

		public static ProjectManager Create(IdentityFactoryOptions<ProjectManager> options,
											IOwinContext context)
		{
			return new ProjectManager(context);
		}

#endregion

#region Own Methods

		public Task<MActionResult<Project>> SelectAsync(Projects project)
		{
			return this.GetByIdAsync((int) project);
		}

#endregion

#endregion
	}
}