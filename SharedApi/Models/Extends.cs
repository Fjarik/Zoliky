using SharedApi.Connectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using SharedLibrary.Enums;
using SharedLibrary.Interfaces;

namespace SharedApi.Models
{
	[Android.Runtime.Preserve(AllMembers = true)]
	public partial class Class : IDbObject, IDbEntity, IClass
	{
		public Class() { }
	}

	[Android.Runtime.Preserve(AllMembers = true)]
	public partial class Crash : IDbObject, IDbEntity
	{
		public Crash() { }
	}

	[Android.Runtime.Preserve(AllMembers = true)]
	public partial class Changelog : IDbObject, IDbEntity
	{
		public Changelog() { }
	}

	[Android.Runtime.Preserve(AllMembers = true)]
	public partial class Image : IDbObject, IDbEntity, IImage
	{
		public Image() { }
	}

	[Android.Runtime.Preserve(AllMembers = true)]
	public partial class News : IDbObject, IDbEntity
	{
		public News() { }
	}

	[Android.Runtime.Preserve(AllMembers = true)]
	public partial class Price : IDbObject, IDbEntity
	{
		public Price() { }
	}

	[Android.Runtime.Preserve(AllMembers = true)]
	public partial class Project : IDbObject, IDbEntity
	{
		public Project() { }
	}

	[Android.Runtime.Preserve(AllMembers = true)]
	public partial class Role : IDbObject, IDbEntity, IRole
	{
		public Role() { }
	}

	[Android.Runtime.Preserve(AllMembers = true)]
	public partial class Rank : IDbObject, IDbEntity
	{
		public int ID { get; set; }
		public string Title { get; set; }
		public int FromXP { get; set; }
		public int ToXP { get; set; }
		public string Colour { get; set; }

		public Rank() { }
	}

	[Android.Runtime.Preserve(AllMembers = true)]
	public partial class Subject : IDbObject, IDbEntity { }

	[Android.Runtime.Preserve(AllMembers = true)]
	public partial class Transaction : IDbObject, IDbEntity, ITransaction
	{
		[CanBeNull]
		public string From { get; set; }

		[CanBeNull]
		public string To { get; set; }

		public string ZolikTitle { get; set; }

		public ZolikType? ZolikType { get; set; }

		public string DirectionFull { get; set; }

		/*
		public TransactionAssignment Typ
		{
			get => (TransactionAssignment)this.Assignment;
			set => this.Assignment = (int)value;
		}
		*/
		public Transaction() { }
	}

	[Android.Runtime.Preserve(AllMembers = true)]
	public partial class Unavailability : IDbObject, IDbEntity
	{
		public Projects Project => (Projects) this.ProjectID;

		public Unavailability() { }
	}

	[Android.Runtime.Preserve(AllMembers = true)]
	public partial class User : IDbObject, IDbEntity, ITokenable, IUser<Class, Image, Role>
	{
		[CanBeNull]
		public DateTime? LastLoginDate { get; set; }

		public Rank Rank { get; set; }

		[JsonIgnore]
		private string _token;

		[JsonIgnore]
		public string Token
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_token)) {
					return "";
				}
				return _token;
			}
			set => _token = value;
		}

		public string FullName => $"{this.Name} {this.Lastname}";

		public string ClassName => this.Class?.Name;

		public List<Zolik> Zoliky { get; set; }

		public string SpecialUsername { get; set; }

		[JsonIgnore]
		public bool IsTesterType
		{
			get
			{
				if (this.SpecialUsername.Contains("_tester")) {
					return true;
				}

				if (this.Type == UserPermission.Dev) {
					return true;
				}
				return false;
			}
			set
			{
				if (value) {
					if (!this.SpecialUsername.Contains("_tester")) {
						this.SpecialUsername = this.Username + "_tester";
					}
				} else {
					if (this.SpecialUsername.Contains("_tester")) {
						this.SpecialUsername = this.SpecialUsername.Replace("_tester", "");
					}
				}
			}
		}

		public User() { }

		public Version GetVersion() => Version.Parse(this.VersionS);

		public bool IsInRole(string role)
		{
			if (this.Roles == null || this.Roles.Count < 1) {
				return false;
			}
			return this.Roles.Any(x => x.Name == role);
		}

		public bool IsInRolesOr(params string[] roles)
		{
			foreach (var role in roles) {
				if (this.IsInRole(role)) {
					return true;
				}
			}
			return false;
		}

		public bool IsInRolesAnd(params string[] roles)
		{
			foreach (var role in roles) {
				if (!this.IsInRole(role)) {
					return false;
				}
			}
			return true;
		}
	}

	[Android.Runtime.Preserve(AllMembers = true)]
	public partial class UserLogin : IDbObject, IDbEntity
	{
		public UserLogin() { }
	}

	[Android.Runtime.Preserve(AllMembers = true)]
	public partial class WebEvent : IDbObject, IDbEntity
	{
		public WebEvent() { }
	}

	[Android.Runtime.Preserve(AllMembers = true)]
	public partial class Zolik : IDbObject, IDbEntity, IZolik
	{
		public bool IsLocked { get; set; }

		public bool CanBeTransfered { get; set; }

		public bool IsSplittable { get; set; }

		public Zolik() { }
	}
}