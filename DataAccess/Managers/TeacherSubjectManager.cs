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

namespace DataAccess.Managers
{
	public class TeacherSubjectManager : BaseManager<TeacherSubject>, IDisposable
	{
		/// 
		/// Fields
		///
		/// 
		/// Constructors
		/// 
		public TeacherSubjectManager(IOwinContext context) : this(context, new ZoliksEntities()) { }

		public TeacherSubjectManager(IOwinContext context, ZoliksEntities ctx) : base(context, ctx) { }

		// Methods

#region Methods

		/// 
		/// Own methods
		/// 

#region Static methods

		public static TeacherSubjectManager Create(IdentityFactoryOptions<TeacherSubjectManager> options,
												   IOwinContext context)
		{
			return new TeacherSubjectManager(context);
		}

#endregion

#region Own Methods

		public Task<TeacherSubject> GetAsync(int teacherId,
											 int subjectId,
											 int classId)
		{
			return _ctx.TeacherSubjects
					   .Where(x => x.TeacherID == teacherId &&
								   x.SubjectID == subjectId &&
								   x.ClassID == classId)
					   .FirstOrDefaultAsync();
		}

		public Task<List<Class>> GetTeacherClassesAsync(int teacherId,
														int subjectId)
		{
			return _ctx.TeacherSubjects
					   .Where(x => x.TeacherID == teacherId &&
								   x.SubjectID == subjectId)
					   .Select(x => x.Class)
					   .ToListAsync();
		}

		public Task<List<int>> GetTeacherClassIdsAsync(int teacherId,
													   int subjectId)
		{
			return _ctx.TeacherSubjects
					   .Where(x => x.TeacherID == teacherId &&
								   x.SubjectID == subjectId)
					   .Select(x => x.ClassID)
					   .ToListAsync();
		}

		public Task<List<Class>> GetTeacherClassesAsync(int teacherId)
		{
			return _ctx.TeacherSubjects
					   .Where(x => x.TeacherID == teacherId)
					   .Select(x => x.Class)
					   .ToListAsync();
		}

		public async Task<MActionResult<TeacherSubject>> CreateAsync(int teacherId,
																	 int subjectId,
																	 int classId)
		{
			if (teacherId < 1 || subjectId < 1 || classId < 1) {
				return new MActionResult<TeacherSubject>(StatusCode.NotValidID);
			}

			var ent = new TeacherSubject {
				TeacherID = teacherId,
				SubjectID = subjectId,
				ClassID = classId,
				Created = DateTime.Now
			};
			return await CreateAsync(ent);
		}

		private async Task<MActionResult<TeacherSubject>> CreateAsync(TeacherSubject sub)
		{
			if (sub == null) {
				return new MActionResult<TeacherSubject>(StatusCode.InvalidInput);
			}
			var ent = _ctx.TeacherSubjects.Add(sub);
			var changes = await base.SaveAsync();
			if (changes == 0) {
				return new MActionResult<TeacherSubject>(StatusCode.InternalError);
			}
			return new MActionResult<TeacherSubject>(StatusCode.OK, ent);
		}

		public async Task<bool> DeleteAsync(int teacherId,
											 int subjectId,
											 int classId)
		{
			var ent = await this.GetAsync(teacherId, subjectId, classId);
			if (ent == null) {
				return false;
			}
			return await base.DeleteAsync(ent);
		}

		public Task<int> DeleteAsync(int teacherId,
									  int subjectId)
		{
			return _ctx.TeacherSubjects
					   .Where(x => x.TeacherID == teacherId &&
								   x.SubjectID == subjectId)
					   .DeleteFromQueryAsync();
		}

#endregion

#endregion
	}
}