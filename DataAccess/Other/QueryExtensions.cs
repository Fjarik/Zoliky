using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using SharedLibrary.Enums;
using SharedLibrary.Interfaces;
using SharedLibrary.Shared.ApiModels;

namespace DataAccess
{
	public static class QueryExtensions
	{
		public static IOrderedQueryable<User> SortStudents(this IQueryable<User> query)
		{
			return query.OrderBy(x => x.Class.Name)
						.ThenBy(x => x.Name)
						.ThenBy(x => x.Lastname);
		}

		public static IQueryable<Unavailability> IsActive(this IQueryable<Unavailability> query)
		{
			var now = DateTime.Now;
			return query.Where(x => x.From <= now && now <= x.To);
		}

		public static IQueryable<UserSetting> AsUserSettings(this IQueryable<UserSetting> query,
															 int userId,
															 string key,
															 int? project = null)
		{
			return query.AsNoTracking()
						.Where(x => x.UserID == userId &&
									x.Key == key &&
									x.ProjectID == project);
		}

		public static IQueryable<ProjectSetting> AsProjectSettings(this IQueryable<ProjectSetting> query,
																   int? projectId,
																   string key)
		{
			return query.AsNoTracking()
						.Where(x => x.ProjectID == projectId &&
									x.Key == key);
		}
	}
}