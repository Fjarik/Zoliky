﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Models;

namespace ApiGraphQL.Repository
{
	public interface IZolikRepository
	{
		IEnumerable<Zolik> GetAll();
		Zolik GetById(int id);
		IEnumerable<Zolik> GetByOwnerId(int userId);
	}
}