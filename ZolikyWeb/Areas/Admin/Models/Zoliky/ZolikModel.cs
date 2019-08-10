using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataAccess.Models;
using SharedLibrary.Enums;
using SharedLibrary.Interfaces;
using SharedLibrary.Shared;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Areas.Admin.Models.Zoliky
{
	public sealed class ZolikModel : UniversalModel<Zolik>
	{
#region Model

		public override bool AllowRemove { get; set; }
		public override bool AllowEdit { get; set; }
		public override bool IsCreate { get; set; }
		public override int PreviousID { get; set; }
		public override int ID { get; set; }
		public override int NextID { get; set; }
		public override string ActionName { get; set; }

		public override bool IsValid { get; }

#endregion

#region Entity

		public int OwnerID { get; set; }
		public int SubjectID { get; set; }
		public int TeacherID { get; set; }
		public int OriginalOwnerID { get; set; }
		public ZolikType Type { get; set; }
		public string Title { get; set; }

		[Display(Name = "Ano/Ne")]
		public bool Enabled { get; set; }

		public DateTime OwnerSince { get; set; }
		public DateTime Created { get; set; }

		[Display(Name = "Ano/Ne")]
		public bool AllowSplit { get; set; }

		public DataAccess.Models.Subject Subject { get; set; }
		public User OriginalOwner { get; set; }
		public ICollection<Transaction> Transactions { get; set; }
		public User Owner { get; set; }
		public User Teacher { get; set; }

#endregion

#region Model lists

		public IEnumerable<ZolikType> ZolikTypes => Enum.GetValues(typeof(ZolikType))
														.Cast<ZolikType>()
														.Where(x => !x.IsTesterType());

		public IEnumerable<SelectListItem> TypeSelect => this.ZolikTypes
															 .Select(x => new SelectListItem {
																 Value = ((int) x).ToString(),
																 Text = x.GetDescription(),
																 Disabled = false
															 });

		public List<DataAccess.Models.Subject> Subjects { get; set; }

		public IEnumerable<SelectListItem> SubjectSelect => this.Subjects
																.Select(x => new SelectListItem {
																	Value = x.ID.ToString(),
																	Text = x.Name,
																	Disabled = false
																});

#endregion

		public ZolikModel() : base() { }

		public ZolikModel(Zolik ent,
						  List<DataAccess.Models.Subject> subjects,
						  bool allowEdit,
						  int previousId,
						  int nextId) : base(ent, allowEdit, previousId, nextId)
		{
			this.ID = ent.ID;
			this.OwnerID = ent.OwnerID;
			this.SubjectID = ent.SubjectID;
			this.TeacherID = ent.TeacherID;
			this.OriginalOwnerID = ent.OriginalOwnerID;
			this.Type = ent.Type;
			this.Title = ent.Title;
			this.Enabled = ent.Enabled;
			this.OwnerSince = ent.OwnerSince;
			this.Created = ent.Created;
			this.AllowSplit = ent.AllowSplit;
			this.Subject = ent.Subject;
			this.OriginalOwner = ent.OriginalOwner;
			this.Transactions = ent.Transactions;
			this.Owner = ent.Owner;
			this.Teacher = ent.Teacher;

			this.Subjects = subjects;
		}
	}
}