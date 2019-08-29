using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiGraphQL.Repository.Interfaces;
using DataAccess.Models;
using GraphQL.DataLoader;
using GraphQL.Types;

namespace ApiGraphQL.GraphQL.Types
{
	public class SchoolType : ObjectGraphType<School>
	{
		public SchoolType(ISchoolRepository schools, IDataLoaderContextAccessor dataLoader)
		{
			Field(x => x.ID, type: typeof(IdGraphType))
				.Description("ID školy");
			Field(x => x.Name)
				.Description("Celý název školy");
			Field(x => x.Street)
				.Description("Celý název školy");
			Field(x => x.City)
				.Description("Celý název školy");
			Field(x => x.AllowTransfer)
				.Description("Celý název školy");
			Field(x => x.AllowTeacherRemove)
				.Description("Celý název školy");
			Field(x => x.AllowZolikSplik)
				.Description("Celý název školy");
			Field<SchoolEnumType>(nameof(School.Type), "Druh školy");
			Field<ListGraphType<SubjectType>>(nameof(School.Subjects),
											  "Předměty vyučované na škole",
											  resolve: x => {
												  var loader =
													  dataLoader
														  .Context
														  .GetOrAddCollectionBatchLoader<int, Subject
														  >("GetSchoolSubjectsBySchoolIds", schools.GetSchoolSubjectsBySchoolIds);
												  return loader.LoadAsync(x.Source.ID);
												  return schools.GetSchoolSubjects(x.Source.ID);
											  });
		}
	}
}