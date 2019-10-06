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
		public override int ID { get; set; }
		public override string ActionName { get; set; }

		public override bool IsValid => (this.ID == -1 || this.ID > 0) &&
										!string.IsNullOrWhiteSpace(this.Title) &&
										this.OwnerID > 0 &&
										this.SubjectID > 0;

#endregion

#region Entity

		[Required(ErrorMessage = "Musíte vybrat vlastníka")]
		[Range(1, int.MaxValue)]
		public int OwnerID { get; set; }

		[Required(ErrorMessage = "Musíte vybrat předmět")]
		[Range(1, int.MaxValue)]
		public int SubjectID { get; set; }

		public int TeacherID { get; set; }
		public int OriginalOwnerID { get; set; }

		[Required(ErrorMessage = "Musíte vybrat typ")]
		public ZolikType Type { get; set; }

		[Required(ErrorMessage = "Musíte zadat, za co byl žolík udělen")]
		[PlaceHolder(Text = "Zadejte, za co byl žolík udělen")]
		[StringLength(200)]
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

		public ZolikRemoveModel RemoveModel { get; set; }

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
																}).Prepend(new SelectListItem {
																	Value = "-1",
																	Text = "Vyberte předmět",
																	Disabled = true,
																	Selected = this.SubjectID == -1
																});

		public List<IUser> Students { get; set; }

		public IEnumerable<SelectListItem> StudentSelect => this.Students
																.OrderBy(x => x.ClassName)
																.ThenBy(x => x.Lastname)
																.ThenBy(x => x.Name)
																.Select(x => new SelectListItem {
																	Value = x.ID.ToString(),
																	Text = $"{x.FullName} ({x.ClassName})",
																	Disabled = false
																}).Prepend(new SelectListItem {
																	Value = "-1",
																	Text = "Vyberte studenta",
																	Disabled = true,
																	Selected = this.OwnerID == -1
																});

#endregion

		public ZolikModel() : base()
		{
			this.AllowRemove = false;
		}

		public static ZolikModel CreateModel(User teacher,
											 List<DataAccess.Models.Subject> subjects,
											 List<IUser> students)
		{
			return new ZolikModel {
				ID = -1,
				OwnerID = -1,
				SubjectID = -1,
				ActionName = "Create",
				AllowEdit = true,
				IsCreate = true,
				Teacher = teacher,
				Subjects = subjects,
				Students = students,
				Type = ZolikType.Normal,
				Enabled = true,
				AllowSplit = true,
				AllowRemove = false
			};
		}

		public ZolikModel(Zolik ent,
						  List<DataAccess.Models.Subject> subjects,
						  bool allowRemove,
						  bool allowEdit,
						  int previousId,
						  int nextId,
						  string url) : base(ent, allowEdit, previousId, nextId, url)
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

			this.AllowRemove = allowRemove && this.Enabled;

			this.Subjects = subjects;

			this.RemoveModel = new ZolikRemoveModel {
				ID = this.ID,
				OwnerID = this.OwnerID,
				Title = this.Title,
				OwnerName = this.Owner.FullName
			};
		}
	}
}