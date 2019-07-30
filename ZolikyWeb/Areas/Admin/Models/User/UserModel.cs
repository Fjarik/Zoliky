using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZolikyWeb.Areas.Admin.Models.User
{
	public class UserModel
	{
		public IEnumerable<DataAccess.Models.User> Users { get; set; }
	}
}