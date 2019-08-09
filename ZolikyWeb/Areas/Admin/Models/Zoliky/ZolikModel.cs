using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataAccess.Models;
using SharedLibrary.Enums;
using SharedLibrary.Interfaces;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Areas.Admin.Models.Zoliky
{
	public class ZolikModel : UniversalModel<Zolik>, IZolik
	{
		public override bool AllowRemove { get; set; }
		public override bool AllowEdit { get; set; }
		public override bool IsCreate { get; set; }
		public override int PreviousID { get; set; }
		public override int ID { get; set; }
		public override int NextID { get; set; }
		public override string ActionName { get; set; }
		public override bool IsValid { get; }
		public bool Enabled { get; set; }
		public int OwnerID { get; set; }
		public int SubjectID { get; set; }
		public int TeacherID { get; set; }
		public int OriginalOwnerID { get; set; }
		public ZolikType Type { get; set; }
		public string Title { get; set; }
		public DateTime OwnerSince { get; set; }
		public DateTime Created { get; set; }
		public string Lock { get; set; }
		public bool IsLocked { get; }
		public bool CanBeTransfered { get; }
		public bool IsSplittable { get; }

		public ZolikModel() : base() { }

		public ZolikModel(Zolik ent,
						  List<IUser> students,
						  List<DataAccess.Models.Subject> subjects,
						  bool allowEdit,
						  int previousId,
						  int nextId) : base(ent, allowEdit, previousId, nextId)
		{

		}
	}
}