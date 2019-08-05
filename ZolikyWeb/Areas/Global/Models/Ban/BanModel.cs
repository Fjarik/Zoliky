using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DataAccess;
using SharedLibrary.Interfaces;

namespace ZolikyWeb.Areas.Global.Models.Ban
{
	[MetadataType(typeof(BanMetadata))]
	public sealed class BanModel : DataAccess.Models.Ban, IValidable
	{
		public bool IsPermanent => this.To == null;

		public override DataAccess.Models.User User { get; set; }

		public BanModel() { }

		public BanModel(DataAccess.Models.Ban b)
		{
			this.ID = b.ID;
			this.UserID = b.UserID;
			this.From = b.From;
			this.To = b.To;
			this.Reason = b.Reason;
			this.User = b.User;
		}

		public bool IsValid => this.ID > 0 &&
							   !string.IsNullOrWhiteSpace(this.Reason) &&
							   (Extensions.IsActive().Compile()(this));

		private sealed class BanMetadata
		{
			[Display(Name = "Důvod")]
			[Required(ErrorMessage = "Musíte zadat důvod")]
			[StringLength(200)]
			public string Reason { get; set; }

			[Display(Name = "Konec")]
			[DataType(DataType.Date)]
			public DateTime? To { get; set; }
		}
	}
}