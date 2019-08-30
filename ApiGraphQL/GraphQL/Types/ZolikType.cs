using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiGraphQL.Repository.Interfaces;
using DataAccess.Models;
using GraphQL.DataLoader;
//using DataAccessCore.Models;
using GraphQL.Types;

namespace ApiGraphQL.GraphQL.Types
{
	public class ZolikType : ObjectGraphType<Zolik>
	{
		public ZolikType(IZolikRepository zoliks, IDataLoaderContextAccessor dataLoader)
		{
			Field("id", x => x.ID, type: typeof(IdGraphType))
				.Description("ID žolíka");
			Field(x => x.OwnerID, type: typeof(IdGraphType))
				.Description("ID aktuálního vlastníka");
			Field(x => x.SubjectID, type: typeof(IdGraphType))
				.Description("ID předmětu, ze kterého byl žolík udělen");
			Field(x => x.TeacherID, type: typeof(IdGraphType))
				.Description("ID učitele, který udělil žolíka");
			Field(x => x.OriginalOwnerID, type: typeof(IdGraphType))
				.Description("ID originálního vlastníka žolíka");
			Field(x => x.SchoolID, type: typeof(IdGraphType))
				.Description("ID školy, pod kterou žolík spadá");
			Field(x => x.Title)
				.Description("Za co byl žolík udělen");
			Field(x => x.Enabled)
				.Description("Udává, zda je žolík aktivní");
			Field(x => x.OwnerSince)
				.Description("Datum, od kdy je žolík u aktuálního vlastníka");
			Field(x => x.Created)
				.Description("Datum vytvoření žolíka");
			Field(x => x.Lock, nullable: true)
				.Description("Za co si přeje uživatel uplatnit žolíka");
			Field(x => x.CanBeTransfered)
				.Description("Udává, zda může být žolík předán jinému studentovi");
			Field(x => x.IsSplittable)
				.Description("Udává, zda je dovoleno žolíka rozdělit");
			Field<ZolikEnumType>(nameof(Zolik.Type), "Typ žolíka");
			Field<SubjectType>(nameof(Zolik.Subject), "Předmět, ze kterého byl žolík udělen");
			Field<ListGraphType<TransactionType>>(nameof(Zolik.Transactions),
												  "Transakce daného žolíka",
												  resolve: x => {
													  var loader = dataLoader.Context
																			 .GetOrAddCollectionBatchLoader<int,
																				 Transaction
																			 >("GetTransactionsByZolikIdsAsync",
																			   zoliks.GetTransactionsByZolikIdsAsync);
													  return loader.LoadAsync(x.Source.ID);
												  });
		}
	}
}