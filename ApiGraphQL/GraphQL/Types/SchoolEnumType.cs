using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Models;
using GraphQL.Types;
using SharedLibrary.Enums;

namespace ApiGraphQL.GraphQL.Types
{
	public class SchoolEnumType : EnumerationGraphType<SchoolTypes>
	{
		public SchoolEnumType()
		{
			Name = nameof(School.Type);
			Description = "Druh školy";
		}
	}
}
