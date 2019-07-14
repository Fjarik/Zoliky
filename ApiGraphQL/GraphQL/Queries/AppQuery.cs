using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiGraphQL.GraphQL.Types;
using ApiGraphQL.Repository;
using GraphQL;
using GraphQL.Types;

namespace ApiGraphQL.GraphQL.Queries
{
	public class AppQuery : ObjectGraphType
	{
		public AppQuery(IZolikRepository repository)
		{
			Field<ListGraphType<ZolikType>>(
											"zoliks",
											arguments: new
												QueryArguments(new QueryArgument<IdGraphType>() {
													Name = "ownerId"
												}),
											resolve: x => {
												if (x.HasArgument("ownerId")) {
													if (!(x.GetArgument<int>("ownerId") is int id)) {
														x.Errors.Add(new ExecutionError("Špatná hodnota ID vlastníka"));
														return null;
													}
													return repository.GetByOwnerId(id);
												}
												return repository.GetAll();
											});
			Field<ZolikType>(
							 "zolik",
							 arguments: new QueryArguments(
														   new QueryArgument<NonNullGraphType<IdGraphType>>
															   {Name = "id"}
														  ),
							 resolve: x => {
								 if (!(x.GetArgument<int>("id") is int id)) {
									 x.Errors.Add(new ExecutionError("Špatná hodnota ID"));
									 return null;
								 }
								 return repository.GetById(id);
							 });
		}
	}
}