using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ApiGraphQL.Repository.Interfaces;
using DataAccess.Models;

namespace ApiGraphQL.Repository
{
	public class SchoolRepository : ISchoolRepository
	{
		private ZoliksEntities _context;

		public SchoolRepository(ZoliksEntities context)
		{
			_context = context;
		}

		public IEnumerable<School> GetAll()
		{
			return _context.Schools
						   .ToList();
		}

		public School GetById(int id)
		{
			return _context.Schools
						   .SingleOrDefault(x => x.ID == id);
		}

		public IEnumerable<School> GetByType(int type)
		{
			return _context.Schools
						   .Where(x => (int) x.Type == type)
						   .ToList();
		}

		public IEnumerable<Subject> GetSchoolSubjects(int schoolId)
		{
			return _context.SchoolSubjects
						   .Where(x => x.SchoolID == schoolId)
						   .Select(x => x.Subject)
						   .ToList();
		}

		public async Task<ILookup<int, Subject>> GetSchoolSubjectsBySchoolIdsAsync(IEnumerable<int> schoolIds)
		{
			var subjects = await _context.SchoolSubjects
										 .Include(x => x.Subject)
										 .Where(x => schoolIds.Contains(x.SchoolID))
										 .ToListAsync();
			return subjects.ToLookup(x => x.SchoolID, x => x.Subject);
		}
	}
}