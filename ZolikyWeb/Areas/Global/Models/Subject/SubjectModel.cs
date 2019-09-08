using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SharedLibrary.Interfaces;
using SharedLibrary.Shared;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Areas.Global.Models.Subject
{
	public sealed class SubjectModel : UniversalModel<DataAccess.Models.Subject>
	{
		public override bool AllowRemove { get; set; } = false;
		public override bool AllowEdit { get; set; }
		public override bool IsCreate { get; set; }
		public override int PreviousID { get; set; }
		public override int ID { get; set; }
		public override int NextID { get; set; }
		public override string ActionName { get; set; }

#region Entity

		[Display(Name = "Název")]
		[Required(ErrorMessage = "Musíte zadat název předmětu")]
		[PlaceHolder(Text = "Zadejte název předmětu")]
		public string Name { get; set; }

		[Display(Name = "Zkratka")]
		[Required(ErrorMessage = "Musíte zadat zkratku předmětu")]
		[PlaceHolder(Text = "Zadejte zkratku předmětu")]
		[StringLength(3)]
		public string Shortcut { get; set; }

#endregion

		public override bool IsValid => (this.ID == -1 || this.ID > 0) &&
										!Methods.AreNullOrWhiteSpace(this.Name, this.Shortcut);

		public SubjectModel() : base()
		{
			this.AllowRemove = false;
		}

		public static SubjectModel CreateModel()
		{
			return new SubjectModel {
				ID = -1,
				ActionName = "Create",
				AllowEdit = true,
				IsCreate = true,
			};
		}

		public SubjectModel(DataAccess.Models.Subject ent,
								bool allowEdit,
								int previousId,
								int nextId) : base(ent, allowEdit, previousId, nextId)
		{
			this.Name = ent.Name;
			this.Shortcut = ent.Shortcut;
			this.AllowRemove = !this.IsCreate &&
							   ent.Name != "Jiný" &&
							   ent.GetTeacherCount() == 0;
		}
	}
}