using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharedLibrary.Interfaces;
using ZolikyWeb.Tools;

namespace ZolikyWeb.Areas.Admin.Models.Class
{
	public class ClassModel : UniversalModel<DataAccess.Models.Class>, IClass
	{
		public override bool AllowRemove { get; set; }
		public override bool AllowEdit { get; set; }
		public override bool IsCreate { get; set; }
		public override int PreviousID { get; set; }
		public override int ID { get; set; }
		public override int NextID { get; set; }
		public override string ActionName { get; set; }

		public override bool IsValid => (this.ID == -1 || this.ID > 0) &&
										SchoolID > 0 &&
										!string.IsNullOrWhiteSpace(this.Name);

#region Entity

		public int SchoolID { get; set; }
		public string Name { get; set; }
		public DateTime Since { get; set; }
		public DateTime Graduation { get; set; }
		public bool Enabled { get; set; }

		public string SchoolName { get; set; }

#endregion

		public ClassModel() : base() { }

		public static ClassModel CreateModel()
		{
			return new ClassModel() {
				ID = -1,
				ActionName = "Create",
				AllowEdit = true,
				IsCreate = true
			};
		}

		public ClassModel(DataAccess.Models.Class ent, bool allowEdit, int previousId, int nextId) :
			base(ent, allowEdit, previousId, nextId)
		{
			this.SchoolID = ent.SchoolID;
			this.Name = ent.Name;
			this.Since = ent.Since;
			this.Graduation = ent.Graduation;
			this.Enabled = ent.Enabled;
			this.SchoolName = ent.School.Name;
		}
	}
}