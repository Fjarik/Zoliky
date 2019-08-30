using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Models;
using GraphQL.Types;
using SharedLibrary.Enums;

namespace ApiGraphQL.GraphQL.Types
{
	public class ZolikEnumType : EnumerationGraphType
	{
		public ZolikEnumType()
		{
			Name = "ZolikTypes";
			Description = "Typ žolíka";

			this.AddValues<SharedLibrary.Enums.ZolikType>();
		}
	}
}