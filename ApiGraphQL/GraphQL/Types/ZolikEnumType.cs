using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessCore.Models;
using GraphQL.Types;

namespace ApiGraphQL.GraphQL.Types
{
	public class ZolikEnumType : EnumerationGraphType<SharedLibrary.Enums.ZolikType>
	{
		public ZolikEnumType()
		{
			Name = nameof(Zolik.Type);
			Description = "Typ žolíka";
		}
	}
}