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
			Field("id", x => x.ID, type: typeof(IdGraphType))
				.Description("ID školy");
			Field(x => x.Name)
				.Description("Celý název školy");
			Field(x => x.Street)
				.Description("Ulice školy");
			Field(x => x.City)
				.Description("Město, kde je škola umístěna");
			Field(x => x.AllowTransfer)
				.Description("Nastavení, zda škola umožňuje darování žolíků mezi studenty");
			Field(x => x.AllowTeacherRemove)
				.Description("Nastavení, zda škola umožňuje odebírání žolíků vyučujícím");
			Field(x => x.AllowZolikSplik)
				.Description("Nastavení, zda škola umožňuje rozdělení žolíků");
			Field<SchoolEnumType>(nameof(School.Type), "Druh školy");
			Field<ListGraphType<SubjectType>>(nameof(School.Subjects),
											  "Předměty vyučované na škole",
											  resolve: x => {
												  var loader =
													  dataLoader
														  .Context
														  .GetOrAddCollectionBatchLoader<int, Subject
														  >("GetSchoolSubjectsBySchoolIdsAsync",
															schools.GetSchoolSubjectsBySchoolIdsAsync);
												  return loader.LoadAsync(x.Source.ID);
												  //return schools.GetSchoolSubjects(x.Source.ID);
											  });
		}
	}
}