using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharedLibrary.Interfaces;
using ZolikyWeb.Tools.Interfaces;

namespace ZolikyWeb.Tools
{
	public abstract class UniversalModel<T> : IModel where T : class, IDbEntity
	{
		public abstract bool AllowRemove { get; set; }
		public abstract bool AllowEdit { get; set; }
		public abstract bool IsCreate { get; set; }
		public abstract int PreviousID { get; set; }
		public abstract int ID { get; set; }
		public abstract int NextID { get; set; }
		public abstract string ActionName { get; set; }
		public virtual bool IsFirst => this.PreviousID == 0;
		public virtual bool IsLast => this.NextID == 0;

		public abstract bool IsValid { get; }

		protected UniversalModel()
		{
			this.AllowRemove = false;
		}

		public UniversalModel(T ent, bool allowEdit, int previousId, int nextId) : this()
		{
			this.AllowEdit = allowEdit;
			this.PreviousID = PreviousID;
			this.ID = ent.ID;
			this.NextID = nextId;
		}
	}
}