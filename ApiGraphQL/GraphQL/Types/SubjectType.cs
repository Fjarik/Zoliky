using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Models;
using GraphQL.Types;

namespace ApiGraphQL.GraphQL.Types
{
	public class SubjectType : ObjectGraphType<Subject>
	{
		public SubjectType()
		{
			Field(x => x.ID, type: typeof(IdGraphType))
				.Description("ID předmětu");
			Field(x => x.Name)
				.Description("Název předmětu");
			Field(x => x.Shortcut)
				.Description("Zkratka předmětu");
			
		}
	}
}
