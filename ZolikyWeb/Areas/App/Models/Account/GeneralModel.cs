using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataAccess.Models;

namespace ZolikyWeb.Areas.App.Models.Account
{
	public class GeneralModel
	{
		public GeneralChangeModel GeneralChange { get; set; }

		public ChangePasswordModel ChangePassword { get; set; }

		public GeneralModel()
		{
		}

		public GeneralModel(User u)
		{
			this.GeneralChange = new GeneralChangeModel(u.Username, u.Email, (int) u.Sex);
			this.ChangePassword = new ChangePasswordModel(u.UQID.ToString());
		}
	}
}