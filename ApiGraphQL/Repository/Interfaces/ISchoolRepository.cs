using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Models;

namespace ApiGraphQL.Repository.Interfaces
{
	public interface ISchoolRepository
	{
		IEnumerable<School> GetAll();
		School GetById(int id);
		IEnumerable<School> GetByType(int type);
		IEnumerable<Subject> GetSchoolSubjects(int schoolId);
		Task<ILookup<int, Subject>> GetSchoolSubjectsBySchoolIds(IEnumerable<int> schoolIds);
	}
}