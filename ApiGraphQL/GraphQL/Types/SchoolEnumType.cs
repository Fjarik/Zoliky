using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Models;
using GraphQL.Types;
using SharedLibrary.Enums;

namespace ApiGraphQL.GraphQL.Types
{
	public class SchoolEnumType : EnumerationGraphType
	{
		public SchoolEnumType()
		{
			Name = "SchoolType";
			Description = "Druh školy";

			this.AddValues<SchoolTypes>();
		}
	}
}