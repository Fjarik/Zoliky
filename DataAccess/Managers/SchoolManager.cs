﻿using System;
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
using SharedLibrary.Shared;

namespace DataAccess.Managers
{
	public class SchoolManager : Manager<School>
	{
		/// 
		/// Fields
		///
		/// 
		/// Constructors
		/// 
		public SchoolManager(IOwinContext context) : this(context, new ZoliksEntities()) { }

		public SchoolManager(IOwinContext context, ZoliksEntities ctx) : base(context, ctx) { }

		// Methods

#region Methods

		/// 
		/// Overrides
		///

#region Overrides

		public override async Task<bool> DeleteAsync(School entity)
		{
			if (entity.Classes.Any() || entity.Users.Any()) {
				return false;
			}

			await _ctx.SchoolSubjects.Where(x => x.SchoolID == entity.ID).DeleteFromQueryAsync();
			return await base.DeleteAsync(entity);
		}

#endregion

		/// 
		/// Own methods
		/// 

#region Static methods

		public static SchoolManager Create(IdentityFactoryOptions<SchoolManager> options,
										   IOwinContext context)
		{
			return new SchoolManager(context);
		}

#endregion

#region Own Methods

		public Task<SchoolTypes> GetSchoolTypeAsync(int schoolId)
		{
			return _ctx.Schools
					   .Where(x => x.ID == schoolId)
					   .Select(x => x.Type)
					   .FirstOrDefaultAsync();
		}

#region Create

#region School subjects

		public Task<List<SchoolSubject>> GetSchoolSubjectsAsync(int schoolId)
		{
			return _ctx.SchoolSubjects
					   .Where(x => x.SchoolID == schoolId)
					   .OrderBy(x => x.Subject.Name)
					   .Include(x => x.Subject)
					   .ToListAsync();
		}

		public Task<SchoolSubject> GetSchoolSubjectAsync(int schoolId, int subjectId)
		{
			return _ctx.SchoolSubjects
					   .Include(x => x.Subject)
					   .FirstOrDefaultAsync(x => x.SchoolID == schoolId &&
												 x.SubjectID == subjectId);
		}

		public Task<SchoolSubject> CreateSchoolSubjectAsync(int schoolId, int subjectId)
		{
			var s = new SchoolSubject {
				SchoolID = schoolId,
				SubjectID = subjectId,
				Bypass = null
			};
			return CreateSchoolSubjectAsync(s);
		}

		private async Task<SchoolSubject> CreateSchoolSubjectAsync(SchoolSubject s)
		{
			var ent = _ctx.SchoolSubjects.Add(s);
			await SaveAsync();
			return await GetSchoolSubjectAsync(ent.SchoolID, ent.SubjectID);
		}

		public Task<int> RemoveSchoolSubjectsAsync(int schoolId, IEnumerable<int> ids)
		{
			return _ctx.SchoolSubjects
					   .Where(x => x.SchoolID == schoolId &&
								   ids.Contains(x.SubjectID))
					   .DeleteFromQueryAsync();
		}

		public async Task<bool> RemoveSchoolSubjectAsync(int schoolId, int subjectId)
		{
			var ent = await GetSchoolSubjectAsync(schoolId, subjectId);
			return await RemoveSchoolSubjectAsync(ent);
		}

		private async Task<bool> RemoveSchoolSubjectAsync(SchoolSubject ent)
		{
			_ctx.SchoolSubjects.Remove(ent);
			await SaveAsync();
			return true;
		}

#endregion

		public Task<List<Subject>> GetSubjectsAsync(int schoolId)
		{
			return _ctx.SchoolSubjects
					   .Include(x => x.Subject)
					   .Where(x => x.SchoolID == schoolId)
					   .Select(x => x.Subject)
					   .OrderBy(x => x.Name)
					   .ToListAsync();
		}

		public async Task<MActionResult<School>> CreateAsync(SchoolTypes type,
															 string name,
															 string street,
															 string city,
															 bool allowTransfer,
															 bool allowTeacherRemove,
															 bool allowSplit)
		{
			if (Methods.AreNullOrWhiteSpace(name, street, city)) {
				return new MActionResult<School>(StatusCode.InvalidInput);
			}

			var ent = new School {
				Type = type,
				Name = name,
				Street = street,
				City = city,
				AllowTransfer = allowTransfer,
				AllowTeacherRemove = allowTeacherRemove,
				AllowZolikSplik = allowSplit
			};
			return await base.CreateAsync(ent);
		}

#endregion

#endregion

#endregion
	}
}