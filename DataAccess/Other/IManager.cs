using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using JetBrains.Annotations;
using SharedLibrary;
using SharedLibrary.Interfaces;

namespace DataAccess
{
	public interface IManager<T> where T : class, IDbObject
	{

		[NotNull]
		MActionResult<T> Select(int id);

		/// <summary>
		/// Saves the specified item
		/// </summary>
		/// <param name="content">The item to save</param>
		/// <param name="throwException">if set to <c>true</c> [throw exception]</param>
		/// <returns>Number of changes</returns>
		/// <exception cref="DbEntityValidationException"></exception>
		[NotNull]
		int Save([CanBeNull]T content, bool throwException = true);

	}
}
