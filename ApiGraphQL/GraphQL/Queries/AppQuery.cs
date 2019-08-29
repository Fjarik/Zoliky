using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiGraphQL.GraphQL.Types;
using ApiGraphQL.Repository;
using ApiGraphQL.Repository.Interfaces;
using GraphQL;
using GraphQL.Types;

namespace ApiGraphQL.GraphQL.Queries
{
	public class AppQuery : ObjectGraphType
	{
		public AppQuery(IZolikRepository zoliks,
						ISchoolRepository schools)
		{
#region Zoliks

			Field<ListGraphType<ZolikType>>(
											"zoliks",
											arguments: new
												QueryArguments(new QueryArgument<IdGraphType>() {
													Name = "ownerId"
												}),
											resolve: x => {
												if (!x.HasArgument("ownerId")) {
													return zoliks.GetAll();
												}
												if (x.GetArgument<int>("ownerId") is int id) {
													return zoliks.GetByOwnerId(id);
												}
												x.Errors.Add(new ExecutionError("Špatná hodnota ID vlastníka"));
												return null;
											});
			Field<ZolikType>(
							 "zolik",
							 arguments: new QueryArguments(
														   new QueryArgument<NonNullGraphType<IdGraphType>>
															   {Name = "id"}
														  ),
							 resolve: x => {
								 if (x.GetArgument<int>("id") is int id) {
									 return zoliks.GetById(id);
								 }
								 x.Errors.Add(new ExecutionError("Špatná hodnota ID"));
								 return null;
							 });

#endregion

#region Schools

			Field<ListGraphType<SchoolType>>(
											 "schools",
											 arguments: new
												 QueryArguments(new QueryArgument<SchoolEnumType>() {
													 Name = "type"
												 }),
											 resolve: x => {
												 if (!x.HasArgument("type")) {
													 return schools.GetAll();
												 }
												 if (x.GetArgument<int>("type") is int type) {
													 return schools.GetByType(type);
												 }
												 x.Errors.Add(new ExecutionError("Špatná druh školy"));
												 return null;
											 });

			Field<SchoolType>(
							  "school",
							  arguments: new QueryArguments(
															new QueryArgument<NonNullGraphType<IdGraphType>>
																{Name = "id"}
														   ),
							  resolve: x => {
								  if (x.GetArgument<int>("id") is int id) {
									  return schools.GetById(id);
								  }
								  x.Errors.Add(new ExecutionError("Špatná hodnota ID"));
								  return null;
							  });

#endregion
		}
	}
}