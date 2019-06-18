using System;
using System.Collections.Generic;

namespace SharedApi.Models
{
	public partial class User
	{
		public int ID { get; set; }
		public Nullable<int> ProfilePhotoID { get; set; }
		public Nullable<int> ClassID { get; set; }
		public System.Guid UQID { get; set; }
		public string Username { get; set; }
		public string Email { get; set; }
		public string Name { get; set; }
		public string Lastname { get; set; }
		public SharedLibrary.Enums.UserPermission Type { get; set; }
		public bool IsEnabled { get; set; }
		public Nullable<System.DateTime> MemberSince { get; set; }
		public string VersionS { get; set; }
		public int XP { get; set; }

		public virtual Class Class { get; set; }
		public virtual Image ProfilePhoto { get; set; }

		public virtual ICollection<Role> Roles { get; set; }
	}
}