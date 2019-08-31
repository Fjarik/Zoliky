using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiGraphQL.GraphQL.Schemas;
using ApiGraphQL.Repository;
using ApiGraphQL.Repository.Interfaces;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Enums;

namespace ApiGraphQL.GraphQL
{
	public static class GraphQlExtensions
	{
		public static IServiceCollection SetProjectRepositories(this IServiceCollection services)
		{
			return services.AddScoped<IZolikRepository, ZolikRepository>()
						   .AddScoped<ISchoolRepository, SchoolRepository>()
						   .AddScoped<ITransactionRepository, TransactionRepository>();
		}

		public static IServiceCollection SetSchemas(this IServiceCollection services)
		{
			return services.AddScoped<AppSchema>();
		}

		public static void AddValues<TEnum>(this EnumerationGraphType enumType)
			where TEnum : struct, IConvertible
		{
			if (!typeof(TEnum).IsEnum) {
				throw new ArgumentException("EnumerationValue must be of Enum type", nameof(TEnum));
			}
			if (!(Enum.GetValues(typeof(TEnum)) is TEnum[] vals)) {
				return;
			}
			foreach (var type in vals) {
				enumType.AddValue(type.ToString().ToUpper(), type.GetDescription(), Convert.ToInt32(type));
			}
		}
	}
}