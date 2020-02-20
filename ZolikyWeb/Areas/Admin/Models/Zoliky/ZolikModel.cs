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
										//this.OwnerID > 0 &&
										this.SubjectID > 0 &&
										!this.IsCreate || (this.StudentIds.Any() &&
														   this.StudentIds.All(x => x > 0)
														  );

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
		public int TypeID { get; set; }

		public ZolikType Type { get; set; }

		[Required(ErrorMessage = "Musíte zadat, za co byl žolík udělen")]
		[PlaceHolder(Text = "Zadejte, za co byl žolík udělen")]
		[StringLength(200)]
		public string Title { get; set; }

		[Display(Name = "Ano/Ne")]
		public bool Enabled { get; set; }

		public DateTime OwnerSince { get; set; }
		public DateTime Created { get; set; }

		public string Lock { get; set; }

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

		public List<int> StudentIds { get; set; }

		public IEnumerable<ZolikType> ZolikTypes { get; }

		public IEnumerable<SelectListItem> TypeSelect => this.ZolikTypes
															 .Select(x => new SelectListItem {
																 Value = (x.ID).ToString(),
																 Text = x.FriendlyName,
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
			this.Students = new List<IUser>();
			this.StudentIds = new List<int>();
		}

		private ZolikModel(IEnumerable<ZolikType> zTypes, bool isTester = false) : this()
		{
			if (!isTester) {
				zTypes = zTypes.Where(x => !x.IsTestType);
			}
			this.ZolikTypes = zTypes;
		}

		public static ZolikModel CreateModel(User teacher,
											 List<ZolikType> types,
											 List<DataAccess.Models.Subject> subjects,
											 List<IUser> students,
											 bool isTester = false)
		{
			return new ZolikModel(types, isTester) {
				ID = -1,
				OwnerID = -1,
				SubjectID = -1,
				ActionName = "Create",
				AllowEdit = true,
				IsCreate = true,
				Teacher = teacher,
				Subjects = subjects,
				Students = students,
				TypeID = types.First().ID,
				Enabled = true,
				AllowSplit = true,
				AllowRemove = false
			};
		}

		public ZolikModel(Zolik ent,
						  IEnumerable<ZolikType> zTypes,
						  List<DataAccess.Models.Subject> subjects,
						  bool allowRemove,
						  bool allowEdit,
						  int previousId,
						  int nextId,
						  bool isTester = false) : base(ent, allowEdit, previousId, nextId)
		{
			this.ID = ent.ID;
			this.OwnerID = ent.OwnerID;
			this.SubjectID = ent.SubjectID;
			this.TeacherID = ent.TeacherID;
			this.OriginalOwnerID = ent.OriginalOwnerID;
			this.TypeID = ent.Type.ID;
			this.Type = ent.Type;
			this.Title = ent.Title;
			this.Enabled = ent.Enabled;
			this.OwnerSince = ent.OwnerSince;
			this.Created = ent.Created;
			this.Lock = ent.Lock;
			this.AllowSplit = ent.AllowSplit;
			this.Subject = ent.Subject;
			this.OriginalOwner = ent.OriginalOwner;
			this.Transactions = ent.Transactions;
			this.Owner = ent.Owner;
			this.Teacher = ent.Teacher;
			this.StudentIds = new List<int> {
				this.OwnerID
			};

			this.AllowRemove = allowRemove && this.Enabled;

			this.Subjects = subjects;

			if (!isTester) {
				zTypes = zTypes.Where(x => !x.IsTestType);
			}
			this.ZolikTypes = zTypes;

			this.RemoveModel = new ZolikRemoveModel {
				ID = this.ID,
				OwnerID = this.OwnerID,
				Title = this.Title,
				OwnerName = this.Owner.FullName,
				Reason = ent.Lock
			};
		}
	}
}