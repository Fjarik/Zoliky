﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Managers.New;
using DataAccess.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using SharedLibrary;
using SharedLibrary.Enums;

namespace DataAccess.Managers
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

		public Task<List<Class>> GetAllAsync(int schoolId, bool onlyActive = false)
		{
			var query = _ctx.Classes
							.Where(x => x.SchoolID == schoolId);

			if (onlyActive) {
				query = query.Where(x => x.Enabled);
			}
			return query.OrderBy(x => x.Name)
						.ThenByDescending(x => x.Since.Year)
						.ThenBy(x => x.Enabled)
						.ToListAsync();
		}

#region Create

		public Task<MActionResult<Class>> CreateAsync(string name,
													  int schoolId,
													  DateTime since)
		{
			return this.CreateAsync(name,
									schoolId,
									since,
									since.AddYears(4));
		}

		public async Task<MActionResult<Class>> CreateAsync(string name,
															int schoolId,
															DateTime since,
															DateTime graduation)
		{
			if (string.IsNullOrWhiteSpace(name)) {
				return new MActionResult<Class>(StatusCode.InvalidInput);
			}

			Class c = new Class() {
				SchoolID = schoolId,
				Name = name,
				Since = since,
				Graduation = graduation,
				Enabled = true
			};
			return await this.CreateAsync(c);
		}

#endregion

		public async Task<MActionResult<Class>> MoveClassAsync(int id)
		{
			if (id < 1) {
				return new MActionResult<Class>(StatusCode.NotValidID);
			}
			var res = await this.GetByIdAsync(id);
			if (!res.IsSuccess) {
				return res;
			}
			return await MoveClassAsync(res.Content);
		}

		private async Task<MActionResult<Class>> MoveClassAsync(Class c)
		{
			if (c == null) {
				return new MActionResult<Class>(StatusCode.InvalidInput);
			}
			var enabled = await this.CheckClassAsync(c);
			if (!enabled) {
				return new MActionResult<Class>(StatusCode.OK, c);
			}

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
			var classes = await _ctx.Classes.Where(x => x.Enabled).Select(x => x.ID).ToListAsync();
			foreach (int id in classes) {
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
			return await CheckClassAsync(res.Content);
		}

		private async Task<bool> CheckClassAsync(Class c)
		{
			if (c == null || !c.Enabled) {
				return false;
			}
			if (c.Graduation < DateTime.Now) {
				await this.DeactivateClassAsync(c);
				return false;
			}
			return true;
		}

		public async Task<bool> CheckClassesAsync()
		{
			var classes = await _ctx.Classes.Where(x => x.Enabled).Select(x => x.ID).ToListAsync();
			foreach (int id in classes) {
				var res = await this.CheckClassAsync(id);
				if (!res) {
					return false;
				}
			}
			return true;
		}

		public async Task DeactivateClassAsync(Class c)
		{
			if (c == null) {
				return;
			}
			c.Enabled = false;
			await this.SaveAsync(c);

			string sql = @"UPDATE [Users] SET [Enabled] = 0 WHERE [ClassID] = @classId";
			var res = await _ctx.Database.ExecuteSqlCommandAsync(sql, new SqlParameter("classId", c.ID));
		}

#endregion

#endregion
	}
}