using System.Collections.Generic;
using DataAccess.Models;

namespace ApiGraphQL.Repository.Interfaces
{
	public interface IZolikRepository
	{
		IEnumerable<Zolik> GetAll();
		Zolik GetById(int id);
		IEnumerable<Zolik> GetByOwnerId(int userId);
	}
}