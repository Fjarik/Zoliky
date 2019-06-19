using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiGraphQL.GraphQL.Queries;
using GraphQL;
using GraphQL.Types;

namespace ApiGraphQL.GraphQL.Schemas
{
	public class AppSchema : Schema
	{
		public AppSchema(IDependencyResolver resolver) : base(resolver)
		{
			Query = resolver.Resolve<AppQuery>();
		}
	}
}