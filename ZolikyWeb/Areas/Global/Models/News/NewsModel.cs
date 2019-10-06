using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SharedLibrary.Shared;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Areas.Global.Models.News
{
	public sealed class NewsModel : UniversalModel<DataAccess.Models.News>
	{
		public override bool AllowRemove { get; set; } = false;
		public override bool AllowEdit { get; set; }
		public override bool IsCreate { get; set; }
		public override int ID { get; set; }
		public override string ActionName { get; set; }

#region Entity

		[Display(Name = "Nadpis")]
		[Required(ErrorMessage = "Musíte zadat nadpis novinky")]
		[PlaceHolder(Text = "Zadejte nadpis novinky")]
		public string Title { get; set; }

		[AllowHtml]
		[Display(Name = "Obsah")]
		[Required(ErrorMessage = "Musíte zadat obsah novinky")]
		[PlaceHolder(Text = "Zadejte obsah novinky")]
		public string Message { get; set; }

		public System.DateTime Created { get; set; }

		public DateTime? Expiration { get; set; }

#region Extends

		public string AuthorName { get; set; }

#endregion

#endregion

		public override bool IsValid => (this.ID == -1 || this.ID > 0) &&
										!Methods.AreNullOrWhiteSpace(this.Title, this.Message) &&
										(this.Expiration == null || this.Expiration > DateTime.Now);

		public NewsModel() : base()
		{
			this.AllowRemove = false;
		}

		public static NewsModel CreateModel()
		{
			return new NewsModel {
				ID = -1,
				ActionName = "Create",
				AllowEdit = true,
				IsCreate = true
			};
		}

		public NewsModel(DataAccess.Models.News ent,
						 bool allowEdit,
						 int previousId,
						 int nextId,
						 string url) : base(ent, allowEdit, previousId, nextId, url)
		{
			this.Title = ent.Title;
			this.Message = ent.Message;
			this.Created = ent.Created;
			this.Expiration = ent.Expiration;
			this.AuthorName = ent.Author.FullName;

			this.AllowRemove = !this.IsCreate;
		}
	}
}