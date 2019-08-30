using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Models;

namespace ApiGraphQL.Repository.Interfaces
{
	public interface ISchoolRepository : IBaseRepository<School>
	{
		IEnumerable<School> GetByType(int type);
		IEnumerable<Subject> GetSchoolSubjects(int schoolId);
		Task<ILookup<int, Subject>> GetSchoolSubjectsBySchoolIdsAsync(IEnumerable<int> schoolIds);
	}
}