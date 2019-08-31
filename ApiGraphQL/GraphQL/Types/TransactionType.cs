using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Models;
using GraphQL.Types;

namespace ApiGraphQL.GraphQL.Types
{
	public class TransactionType : ObjectGraphType<Transaction>
	{
		public TransactionType()
		{
			Name = "Transakce";
			Description = "Transakce, kdykoliv byl převeden žolík mezi uživateli";

			Field("id", x => x.ID, type: typeof(IdGraphType))
				.Description("ID transakce");
			Field(x => x.FromID, type: typeof(IdGraphType))
				.Description("ID uživatele od kterého byl žolík odeslán");
			Field(x => x.ToID, type: typeof(IdGraphType))
				.Description("ID uživatele kterému byl žolík odeslán");
			Field(x => x.ZolikID, type: typeof(IdGraphType))
				.Description("ID posílaného žolíka");
			Field(x => x.Date)
				.Description("Datum vytvoření transakce");
			Field(x => x.Message, nullable: true)
				.Description("Zpráva transakce");
			Field(x => x.From)
				.Description("Jméno uživatele, od kterého byl žolík odeslán");
			Field(x => x.To)
				.Description("Jméno uživatele kterému byl žolík odeslán");
			Field<TransactionEnumType>(nameof(Transaction.Typ), "Druh transakce");
			Field<ZolikType>(nameof(Transaction.Zolik), "Poslaný žolík");
		}
	}
}