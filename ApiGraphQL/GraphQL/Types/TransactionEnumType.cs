using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Models;
using GraphQL.Types;
using SharedLibrary.Enums;

namespace ApiGraphQL.GraphQL.Types
{
	public class TransactionEnumType : EnumerationGraphType
	{
		public TransactionEnumType()
		{
			Name = "TransactionTypes";
			Description = "Druh transakce";

			this.AddValues<TransactionAssignment>();
		}
	}
}