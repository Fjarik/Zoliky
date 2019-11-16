using System.Data.Entity;
using System.Threading.Tasks;
using DataAccess.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using SharedLibrary;
using SharedLibrary.Enums;
using SharedLibrary.Shared;

namespace DataAccess.Managers
{
	public class SubjectManager : Manager<Subject>
	{
		/// 
		/// Fields
		///
		/// 
		/// Constructors
		/// 
		public SubjectManager(IOwinContext context) : this(context, new ZoliksEntities()) { }

		public SubjectManager(IOwinContext context, ZoliksEntities ctx) : base(context, ctx) { }

		// Methods

#region Methods

		/// 
		/// Own methods
		/// 

#region Static methods

		public static SubjectManager Create(IdentityFactoryOptions<SubjectManager> options, IOwinContext context)
		{
			return new SubjectManager(context);
		}

#endregion

#region Own Methods

		public async Task<MActionResult<Subject>> CreateAsync(string name,
															  string shortcut)
		{
			if (Methods.AreNullOrWhiteSpace(name, shortcut)) {
				return new MActionResult<Subject>(StatusCode.InvalidInput);
			}
			if (await this.ExistsAsync(name) || await this.ExistsAsync(shortcut)) {
				return new MActionResult<Subject>(StatusCode.AlreadyExists);
			}

			Subject i = new Subject() {
				Name = name,
				Shortcut = shortcut
			};
			return await this.CreateAsync(i);
		}

		public async Task<bool> ExistsAsync(string nameOrShortcut)
		{
			if (string.IsNullOrWhiteSpace(nameOrShortcut)) {
				return false;
			}
			nameOrShortcut = nameOrShortcut.Trim().ToLower();
			return await _ctx.Subjects.AnyAsync(x => x.Name.ToLower() == nameOrShortcut ||
													 x.Shortcut.ToLower() == nameOrShortcut);
		}

#endregion

#endregion
	}
}