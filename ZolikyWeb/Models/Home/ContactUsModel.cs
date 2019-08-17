using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using SharedLibrary.Interfaces;
using SharedLibrary.Shared;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Models.Home
{
	public class ContactUsModel : IValidable
	{
		[Display(Name = "Jméno")]
		[DataType(DataType.Text)]
		[Required(ErrorMessage = "Musíte vyplnit jméno")]
		[PlaceHolder(Text = "Zadejte Vaše jméno")]
		[StringLength(70, MinimumLength = 2, ErrorMessage = "Jméno musí být dlouhé minimálně {2} znaky")]
		[RegularExpression(Ext.NameRegEx, ErrorMessage = "Jméno není ve správném formátu")]
		public string Name { get; set; }

		[Display(Name = "Email")]
		[DataType(DataType.EmailAddress, ErrorMessage = "Musíte zadat platný email")]
		[Required(ErrorMessage = "Musíte vyplnit email")]
		[EmailAddress(ErrorMessage = "Musíte zadat platný email")]
		[PlaceHolder(Text = "Zadejte Váš email")]
		[StringLength(300, MinimumLength = 6, ErrorMessage = "Email musí být dlouhý minimálně {2} znaků")]
		public string Email { get; set; }

		[Display(Name = "Zpráva")]
		[DataType(DataType.MultilineText)]
		[Required(ErrorMessage = "Musíte zadat zprávu")]
		[PlaceHolder(Text = "Zadejte zprávu...")]
		[StringLength(500, MinimumLength = 20,
			ErrorMessage = "Zpráva musí být dlouhá minimálně {2} a maximálně {1} znaků")]
		public string Message { get; set; }

		public bool IsValid => !Methods.AreNullOrWhiteSpace(this.Name,
															this.Email,
															this.Message) &&
							   Methods.IsEmailValid(this.Email) &&
							   Regex.Match(this.Name, Ext.NameRegEx).Success;
	}
}