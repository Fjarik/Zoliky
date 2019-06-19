using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiGraphQL.GraphQL.Types;
using ApiGraphQL.Repository;
using GraphQL.Types;

namespace ApiGraphQL.GraphQL.Queries
{
	public class AppQuery : ObjectGraphType
	{
		public AppQuery(IZolikRepository repository)
		{
			Field<ListGraphType<ZolikType>>(
											"zoliks",
											resolve: x => repository.GetAll()
										   );
		}
	}
}