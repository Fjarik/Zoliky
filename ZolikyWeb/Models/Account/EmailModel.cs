using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Models.Account
{
	public abstract class EmailModel 
	{
		[Required(ErrorMessage = "Musíte zadat email")]
		[EmailAddress(ErrorMessage = "Musíte zadat platný email")]
		[PlaceHolder(Text = "Zadejte email")]
		[Display(Name = "Email")]
		public string Email { get; set; }

		public virtual bool IsValid
		{
			get
			{
				if (string.IsNullOrWhiteSpace(this.Email)) {
					return false;
				}
				return new EmailAddressAttribute().IsValid(this.Email);
			}
		}
	}
}