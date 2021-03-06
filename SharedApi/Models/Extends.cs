﻿using SharedApi.Connectors;
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
	public partial class Achievement : IDbEntity
	{
		public Achievement() { }
	}

	public partial class Ban : IDbEntity
	{
		public bool IsActive { get; set; }
	}

	public partial class Class : IClass
	{
		public Class() { }
	}

	public partial class Image : IImage
	{
		public Image() { }
	}

	public partial class News : IDbEntity
	{
		public News() { }
	}

	public partial class Project : IDbEntity
	{
		public Project() { }
	}

	public partial class Rank : IDbEntity { }

	public partial class Role : IRole
	{
		public Role() { }
	}

	public partial class School : IDbEntity { }

	public partial class Subject : IDbEntity { }

	public partial class Transaction : ITransaction
	{
		[CanBeNull]
		public string From { get; set; }

		[CanBeNull]
		public string To { get; set; }

		public string ZolikTitle { get; set; }

		public int ZolikTypeID { get; set; }

		public string DirectionFull { get; set; }

		public string TypeString => this.Typ.GetDescription();

		public Transaction() { }
	}

	public partial class User : ITokenable, IUser<Class, Image, Role>
	{
		[CanBeNull]
		public DateTime? LastLoginDate { get; set; }

		public Rank Rank { get; set; }

		public string FullName => $"{this.Name} {this.Lastname}";

		public string ClassName => this.Class?.Name;

		public string SchoolName { get; set; }

#region Hidden

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

#endregion

		public User() { }

#region Methods

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

#endregion
	}

	public partial class UserLogin : IDbEntity
	{
		public string StatusText => this.Status.GetDescription();
		public string ProjectText => ((Projects) ProjectID).GetDescription();

		public UserLogin() { }
	}

	public partial class UserSetting : IUserSetting { }

	public partial class Zolik : IZolik<ZolikType>
	{
		public string TypeText => this.Type.FriendlyName;
		public string SubjectName { get; set; }
		public string TeacherName { get; set; }

		public int SchoolID { get; set; }

		public bool IsLocked { get; set; }
		public bool IsUnlocked => !IsLocked;

		public bool CanBeTransfered { get; set; }

		public bool IsSplittable { get; set; }

		public Zolik() { }
	}
}