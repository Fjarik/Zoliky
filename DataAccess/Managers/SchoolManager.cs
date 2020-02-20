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
using SharedLibrary.Interfaces;
using SharedLibrary.Shared;
using SharedLibrary.Shared.ApiModels;

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

#region Stats

		public Task<int> GetSchoolCountAsync()
		{
			return _ctx.Schools
					   //.Where(x=> x.Enabled)
					   .CountAsync();
		}

		public Task<int> GetStudentCountAsync()
		{
			return _ctx.Users
					   .Where(x => x.Enabled &&
								   x.ClassID != null &&
								   x.Roles.Any(y => y.Name == UserRoles.Student) &&
								   x.Roles.All(y => y.Name != UserRoles.HiddenStudent &&
													y.Name != UserRoles.FakeStudent))
					   .CountAsync();
		}

		public Task<int> GetStudentCountAsync(int schoolId)
		{
			return _ctx.Users
					   .Where(x => x.SchoolID == schoolId &&
								   x.Enabled &&
								   x.ClassID != null &&
								   x.Roles.Any(y => y.Name == UserRoles.Student) &&
								   x.Roles.All(y => y.Name != UserRoles.HiddenStudent &&
													y.Name != UserRoles.FakeStudent))
					   .CountAsync();
		}

		public Task<int> GetTeacherCountAsync()
		{
			return _ctx.Users
					   .Where(x => x.Enabled &&
								   x.ClassID == null &&
								   x.Roles.All(y => y.Name != UserRoles.Student &&
													y.Name != UserRoles.Administrator) &&
								   x.Roles.Any(y => y.Name == UserRoles.Teacher))
					   .CountAsync();
		}

		public Task<int> GetTeacherCountAsync(int schoolId)
		{
			return _ctx.Users
					   .Where(x => x.SchoolID == schoolId &&
								   x.Enabled &&
								   x.ClassID == null &&
								   x.Roles.All(y => y.Name != UserRoles.Student &&
													y.Name != UserRoles.Administrator) &&
								   x.Roles.Any(y => y.Name == UserRoles.Teacher))
					   .CountAsync();
		}

		public Task<int> GetZolikCountAsync()
		{
			return GetZoliksQuery().CountAsync();
		}

		public Task<int> GetZolikCountAsync(int schoolId)
		{
			return GetZoliksQuery(schoolId).CountAsync();
		}

		public Task<List<Zolik>> GetZoliksAsync(int schoolId)
		{
			return GetZoliksQuery(schoolId).ToListAsync();
		}

		public async Task<List<ZolikTypesData>> GetZoliksGraphDataAsync(int schoolId)
		{
			var res = await GetZoliksQuery(schoolId)
							.Select(x => new {
								x.Type
							})
							.GroupBy(x => x.Type)
							.ToListAsync();

			return res.Select(x => new ZolikTypesData {
						  Label = x.Key.FriendlyName,
						  Count = x.Count()
					  })
					  .ToList();
		}

		private IQueryable<Zolik> GetZoliksQuery()
		{
			return _ctx.Zoliky
					   .Where(x => x.Owner.Roles.All(y => y.Name != UserRoles.HiddenStudent) &&
								   !x.Type.IsTestType &&
								   x.Enabled);
		}

		private IQueryable<Zolik> GetZoliksQuery(int schoolId)
		{
			return _ctx.Zoliky
					   .Where(x => x.OriginalOwner.SchoolID == schoolId &&
								   x.Owner.Roles.All(y => y.Name != UserRoles.HiddenStudent) &&
								   !x.Type.IsTestType &&
								   x.Enabled);
		}

		public async Task<IEnumerable<ClassLeaderboard>> GetClassLeaderboardAsync(int schoolId)
		{
			var students = await this._ctx.Users.Where(x => x.Enabled &&
															x.ClassID != null &&
															x.SchoolID == schoolId)
									 .GroupBy(x => x.ClassID)
									 .Select(x => new {
										 classId = x.Key,
										 count = x.Select(y => y.OriginalZoliks
																.Where(z => !z.Type.IsTestType)
																.Count(z => z.Enabled)).Sum()
									 })
									 .ToListAsync();

			var classes = await this._ctx.Classes
									.Where(x => x.Enabled &&
												x.SchoolID == schoolId)
									.ToListAsync();

			var res = from s in students
					  join c in classes on s.classId equals c.ID into g
					  select new ClassLeaderboard(g.Any() ? g.First() : null, s.count);
			return res.OrderByDescending(x => x.ZolikCount);
		}

#endregion

		public Task<SchoolTypes> GetSchoolTypeAsync(int schoolId)
		{
			return _ctx.Schools
					   .Where(x => x.ID == schoolId)
					   .Select(x => x.Type)
					   .FirstOrDefaultAsync();
		}

		public async Task<List<ZolikType>> GetSchoolZolikTypesAsync(int schoolId, bool isTester = false)
		{
			var school = await _ctx.Schools
								   .Select(x => new {
									   ID = x.ID,
									   types = x.ZolikTypes,
								   })
								   .FirstAsync(x => x.ID == schoolId);
			if (isTester) {
				return school.types.ToList();
			}
			return school.types.Where(x => !x.IsTestType).ToList();
		}

		public async Task<List<User>> GetSchoolTeachersAsync(int schoolId, bool onlyActive = true)
		{
			if (schoolId < 1) {
				return new List<User>();
			}

			var query = _ctx.Users.Where(x => x.SchoolID == schoolId &&
											  x.ID != 22 &&
											  x.Roles.Any(y => y.Name == UserRoles.Teacher ||
															   y.Name == UserRoles.SchoolManager));

			if (onlyActive) {
				query = query.Where(x => x.Enabled);
			}

			return await query.ToListAsync();
		}

		public Task<List<User>> GetStudentsAsync(int schoolId, params int[] excludeIds)
		{
			return _ctx.Users
					   .OnlyVisibleStudents()
					   .Where(x => x.SchoolID == schoolId &&
								   // x.Enabled &&
								   excludeIds.All(y => x.ID != y))
					   .ToListAsync();
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

		public Task<List<Class>> GetClassesAsync(int schoolId)
		{
			return _ctx.Classes
					   .Where(x => x.SchoolID == schoolId &&
								   x.Enabled)
					   .OrderBy(x => x.Name)
					   .ToListAsync();
		}

		public Task<List<Subject>> GetSubjectsAsync(int schoolId)
		{
			return _ctx.SchoolSubjects
					   .Include(x => x.Subject)
					   .Where(x => x.SchoolID == schoolId)
					   .Select(x => x.Subject)
					   .OrderBy(x => x.Name)
					   .ToListAsync();
		}

		public Task<List<Subject>> GetSubjectsByTeacherAsync(int teacherId)
		{
			return _ctx.TeacherSubjects
					   .Include(x => x.Subject)
					   .Where(x => x.TeacherID == teacherId)
					   .Select(x => x.Subject)
					   .Distinct()
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