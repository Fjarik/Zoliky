using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataAccess.Models;
using Microsoft.Owin.Security;

namespace ZolikyWeb.Areas.App.Models.Account
{
	public class ExternalModel
	{
		private IList<AuthenticationDescription> _allProviders;

		public IList<AuthenticationDescription> AllProviders
		{
			get
			{
				if (_allProviders == null) {
					SetAllProviders();
				}
				return _allProviders;
			}
			set => _allProviders = value;
		}

		public IList<UserLoginToken> UserActiveProvider { get; set; }

		private void SetAllProviders()
		{
			_allProviders = HttpContext.Current
									   .GetOwinContext()
									   .Authentication
									   .GetExternalAuthenticationTypes()
									   .ToList();
		}

		public ExternalModel(IList<UserLoginToken> tokens)
		{
			SetAllProviders();
			this.UserActiveProvider = tokens;
		}
	}
}