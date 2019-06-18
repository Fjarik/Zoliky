using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using JetBrains.Annotations;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using SharedLibrary;
using SharedLibrary.Enums;

namespace DataAccess.Managers.New
{
	public class ClassManager : Manager<Class>
	{
		/// 
		/// Fields
		///
		/// 
		/// Constructors
		/// 
		public ClassManager(IOwinContext context) : this(context, new ZoliksEntities()) { }

		public ClassManager(IOwinContext context, ZoliksEntities ctx) : base(context, ctx) { }

		// Methods

#region Methods

		/// 
		/// Overrides
		///

#region Overrides

		public override Task<List<Class>> GetAllAsync()
		{
			return _ctx.Classes
					   .Where(x => x.Enabled)
					   .OrderBy(x => x.Name)
					   .ToListAsync();
		}

#endregion

		/// 
		/// Own methods
		/// 

#region Static methods

		public static ClassManager Create(IdentityFactoryOptions<ClassManager> options,
										  IOwinContext context)
		{
			return new ClassManager(context);
		}

#endregion

#region Own Methods

		public async Task<MActionResult<Class>> CreateAsync(string name, DateTime since)
		{
			if (string.IsNullOrWhiteSpace(name)) {
				return new MActionResult<Class>(StatusCode.InvalidInput);
			}

			Class c = new Class() {
				Name = name,
				Since = since,
				Graduation = since.AddYears(4),
				Enabled = true
			};
			return await this.CreateAsync(c);
		}

		public async Task<MActionResult<Class>> MoveClassAsync(int id)
		{
			if (id < 1) {
				return new MActionResult<Class>(StatusCode.NotValidID);
			}
			var res = await this.GetByIdAsync(id);
			if (!res.IsSuccess) {
				return res;
			}
			var c = res.Content;

			var names = c.Name.Split('.');
			if (names.Length != 2 ||
				!int.TryParse(names[0], out int classNumber) ||
				string.IsNullOrWhiteSpace(names[1])) {
				return new MActionResult<Class>(StatusCode.SeeException,
												new FormatException("Class name is not in right format"));
			}
			classNumber++;

			c.Name = $"{classNumber}.{names[1]}";
			await this.SaveAsync(c);
			return new MActionResult<Class>(StatusCode.OK, c);
		}

		public async Task<bool> MoveClassesAsync()
		{
			foreach (int id in _ctx.Classes.Where(x => x.Enabled).Select(x => x.ID)) {
				var res = await this.MoveClassAsync(id);
				if (!res.IsSuccess) {
					return false;
				}
			}
			return true;
		}

		public async Task<bool> CheckClassAsync(int id)
		{
			if (id < 1) {
				return false;
			}
			var res = await this.GetByIdAsync(id);
			if (!res.IsSuccess) {
				return false;
			}
			var c = res.Content;
			if (!c.Enabled) {
				return false;
			}
			if (c.Graduation < DateTime.Now) {
				c.Enabled = false;
				await this.SaveAsync(c);
				return false;
			}
			return true;
		}

		public async Task<bool> CheckClassesAsync()
		{
			foreach (int id in _ctx.Classes.Where(x => x.Enabled).Select(x => x.ID)) {
				var res = await this.CheckClassAsync(id);
				if (!res) {
					return false;
				}
			}
			return true;
		}

#endregion

#endregion
	}
}