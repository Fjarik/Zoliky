using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using SharedLibrary.Enums;
using SharedLibrary.Interfaces;
using SharedLibrary.Shared;
using DateTime = System.DateTime;

namespace DataAccess.Models
{
#region Context

	public partial class ZoliksEntities
	{
		public ZoliksEntities(string cString) : base(cString) { }

		public static ZoliksEntities Create()
		{
			return new ZoliksEntities();
		}

#endregion

#region Procedures

		public IList<Unavailability> GetUnavailabilities(Projects project, DateTime? date = null)
		{
			if (date == null) {
				date = DateTime.Now;
			}
			return this.GetUnavailabilities((int?) project, date).ToList();
		}

		public IList<GetTopStudents_Result> GetFakeStudents(bool onlyActive = true,
															int? imageMaxSize = null,
															params int[] excludeIds)
		{
			return this.GetFakeStudents(onlyActive: onlyActive,
										imageMaxSize: imageMaxSize)
					   .Where(x => excludeIds.All(y => x.ID != y))
					   .ToList();
		}

		private IEnumerable<GetTopStudents_Result> GetFakeStudents(bool onlyActive,
																   int? imageMaxSize = null)
		{
			if (imageMaxSize == null) {
				imageMaxSize = 1024 * 1024;
			}
			return GetFakeStudents(imageMaxSize: imageMaxSize,
								   defaultPhotoId: Ext.DefaultProfilePhotoId,
								   onlyActive: onlyActive);
		}

		public IList<GetTopStudents_Result> GetStudents(bool onlyActive,
														int? imageMaxSize = null,
														int? classId = null,
														params int[] excludeIds)
		{
			return this.GetStudents(onlyActive: onlyActive,
									imageMaxSize: imageMaxSize,
									classId: classId)
					   .Where(x => excludeIds.All(y => x.ID != y))
					   .ToList();
		}

		private IEnumerable<GetTopStudents_Result> GetStudents(bool onlyActive,
															   int? imageMaxSize = null,
															   int? classId = null)
		{
			if (imageMaxSize == null) {
				imageMaxSize = 1024 * 1024;
			}
			return GetStudents(imageMaxSize: imageMaxSize,
							   classId: classId,
							   defaultPhotoId: Ext.DefaultProfilePhotoId,
							   onlyActive: onlyActive);
		}

		public IList<GetTopStudents_Result> GetTopStudents(int top = 5,
														   int? classId = null,
														   int? imageMaxSize = null)
		{
			return GetTopStudents(key: SettingKeys.LeaderboardZolik,
								  top: top,
								  imageMaxSize: imageMaxSize,
								  classId: classId).ToList();
		}

		private IEnumerable<GetTopStudents_Result> GetTopStudents(string key,
																  int top = 5,
																  int? imageMaxSize = null,
																  int? classId = null)
		{
			if (imageMaxSize == null) {
				imageMaxSize = 1024 * 1024;
			}
			return GetTopStudents(top: top,
								  imageMaxSize: imageMaxSize,
								  classId: classId,
								  settingsKey: key,
								  defaultPhotoId: Ext.DefaultProfilePhotoId);
		}

		public IList<GetTopStudents_Result> GetTopStudentsXp(int top = 5,
															 int? classId = null,
															 int? imageMaxSize = null)
		{
			return GetTopStudentsXp(key: SettingKeys.LeaderboardXp,
									top: top,
									imageMaxSize: imageMaxSize,
									classId: classId).ToList();
		}

		private IEnumerable<GetTopStudents_Result> GetTopStudentsXp(string key,
																	int top = 5,
																	int? imageMaxSize = null,
																	int? classId = null)
		{
			if (imageMaxSize == null) {
				imageMaxSize = 1024 * 1024;
			}
			return GetTopStudentsXp(top: top,
									imageMaxSize: imageMaxSize,
									classId: classId,
									settingsKey: key,
									defaultPhotoId: Ext.DefaultProfilePhotoId);
		}
	}

#endregion

#region Support

	public partial class AnonymTicket : IDbEntity
	{
		public DateTime LastActivity
		{
			get
			{
				if (this.Comments.Count < 1) {
					return this.Created;
				}
				return this.Comments.OrderByDescending(x => x.Created).Select(x => x.Created).First();
			}
		}
	}

	public partial class AnonymTicketComment : IDbEntity
	{
		public string Name => (this.Admin == null) ? this.Ticket?.Name : this.Admin.FullName;
		public bool IsFromAdmin => this.UserId != null;
	}

	public partial class Ticket : IDbEntity, ITicket
	{
		public string Name => this.User?.FullName;
		public string Email => this.User?.Email;

		public DateTime LastActivity
		{
			get
			{
				if (this.Comments.Count < 1) {
					return this.Created;
				}
				return this.Comments.OrderByDescending(x => x.Created).Select(x => x.Created).First();
			}
		}
	}

	public partial class TicketCategory : IDbEntity { }

	public partial class TicketComment : IDbEntity
	{
		public string Name => this.User?.FullName;

		public bool IsFromAdmin
		{
			get
			{
				if (this.Ticket == null) {
					return false;
				}
				return this.Ticket.UserId != this.UserId;
			}
		}
	}

	public partial class FaQ : IDbEntity { }

	public partial class FaqCategory : IDbEntity { }

#endregion

	public partial class Achievement : IDbObject, IDbEntity
	{
		public Achievement() { }
	}

	public partial class AchievementUnlock : IDbObject { }

	[MetadataType(typeof(ClassMetaData))]
	public partial class Class : IDbObject, IDbEntity, IClass
	{
		private sealed class ClassMetaData
		{
			[JsonIgnore]
			public School School { get; set; }
		}
	}

	public partial class GetTopStudents_Result : IDbEntity, IStudent<IImage>
	{
		public string FullName => $"{this.Name} {this.Lastname}";

		private IImage _image { get; set; }

		private IImage GetImage()
		{
			if (this.ImageID == null || this.Size == null) {
				return null;
			}
			return new Image() {
				ID = (int) this.ImageID,
				OwnerID = this.ID,
				Hash = this.Hash,
				Base64 = this.Base64,
				MIME = this.MIME,
				Size = (int) this.Size,
			};
		}

		public IImage ProfilePhoto
		{
			get => this._image ?? (this._image = GetImage());
			set => this._image = value;
		}

		public GetTopStudents_Result() { }
	}

	public partial class Changelog : IDbObject, IDbEntity { }

	[MetadataType(typeof(ImageMetadata))]
	public partial class Image : IDbObject, IDbEntity, IImage
	{
		private sealed class ImageMetadata
		{
			[JsonIgnore]
			public string Name { get; set; }

			[JsonIgnore]
			public DateTime Uploaded { get; set; }

			public string Base64 { get; set; }

			public string MIME { get; set; }
		}
	}

	public partial class News : IDbObject, IDbEntity { }

	public partial class Notification : IDbObject, IDbEntity
	{
		public NotificationType Type
		{
			get
			{
				if (ToID == null && ProjectID == null) {
					return NotificationType.Global;
				}

				if (ProjectID != null) {
					return NotificationType.Project;
				}

				return NotificationType.User;
			}
		}

		public bool IsExpired
		{
			get
			{
				if (Expiration == null) {
					return false;
				}

				return Expiration < DateTime.Now;
			}
		}
	}

	public partial class Project : IDbObject, IDbEntity
	{
		[JsonIgnore]
		public Version VersionV => System.Version.Parse(this.Version);
	}

	public partial class Rank : IDbObject, IDbEntity { }

	public partial class ReadNotification : IDbObject { }

	[MetadataType(typeof(RoleMetadata))]
	public partial class Role : IDbObject, IDbEntity, IRole
	{
		private sealed class RoleMetadata
		{
			[JsonIgnore]
			public ICollection<User> Users { get; set; }
		}
	}

	public partial class Password : IDbObject, IDbEntity
	{
		[JsonIgnore]
		public Version Verze => System.Version.Parse(this.Version);

		public bool IsExpired
		{
			get
			{
				if (this.Expiration == null) {
					return false;
				}
				return (this.Expiration < DateTime.Now);
			}
		}
	}

	[MetadataType(typeof(SchoolMetaData))]
	public partial class School : IDbObject, IDbEntity
	{
		private sealed class SchoolMetaData
		{
			[JsonIgnore]
			public ICollection<Class> Classes { get; set; }

			[JsonIgnore]
			public ICollection<User> Users { get; set; }
		}
	}

	public partial class Subject : IDbObject, IDbEntity { }

	public partial class TeacherSubject : IDbObject { }

	public partial class Token : IDbObject, IDbEntity { }

	[MetadataType(typeof(TransactionMetaData))]
	public partial class Transaction : IDbObject, IDbEntity, ITransaction
	{
		public string From => this.fromUser?.FullName;

		public string To => this.toUser?.FullName;

		public string ZolikTitle => this.Zolik?.Title;

		public ZolikType ZolikType => Zolik?.Type ?? ZolikType.Normal;

		private sealed class TransactionMetaData
		{
			[JsonIgnore]
			public Zolik Zolik { get; set; }
		}
	}

	public partial class Unavailability : IDbObject, IDbEntity
	{
		public Projects? Project => (Projects?) ProjectID;
	}

	[MetadataType(typeof(UserMetaData))]
	public partial class User : IDbObject, IDbEntity, IEnableable, IStudent<Image>, IUser<Class, Image, Role>
	{
#region Get only

		public string ClassName => this.Class?.Name;

		public string SchoolName => this.School.Name;

		[JsonIgnore]
		public string FullName
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				if (!string.IsNullOrWhiteSpace(this.Name)) {
					sb.Append(this.Name);
					sb.Append(" ");
				}
				if (!string.IsNullOrWhiteSpace(this.Lastname)) {
					sb.Append(this.Lastname);
				}
				return sb.ToString();
			}
		}

		[JsonIgnore]
		public UserLogin LastLogin => this.Logins
										  .OrderByDescending(x => x.Date)
										  .FirstOrDefault();

		public DateTime? LastLoginDate => this.LastLogin?.Date;

		public bool IsEnabled
		{
			get
			{
				if (this.Class?.Enabled == false) {
					this.Enabled = false;
				}
				return this.Enabled;
			}
		}

		public bool IsVisiblePublic => !this.IsInRolesOr(UserRoles.HiddenStudent, UserRoles.FakeStudent);

#endregion

#region Get, Set

		[JsonIgnore]
		public System.Version Version
		{
			get => System.Version.Parse(this.VersionS);
			set => this.VersionS = value.ToString();
		}

#endregion

#region Methods

		public bool IsInRole(string role)
		{
			if (this.Roles == null || this.Roles.Count < 1) {
				return false;
			}
			return this.Roles.Any(x => x.Name == role);
		}

		public bool IsInRolesOr(params string[] roles)
		{
			return roles.Any(this.IsInRole);
		}

		public bool IsInRolesAnd(params string[] roles)
		{
			return roles.All(this.IsInRole);
		}

#endregion

		private sealed class UserMetaData
		{
			[JsonIgnore]
			public int? ProfilePhotoID { get; set; }

			[JsonIgnore]
			public int? PasswordID { get; set; }

			[JsonIgnore]
			public bool Enabled { get; set; }

			[JsonIgnore]
			public string RegistrationIp { get; set; }

			[JsonIgnore]
			public string Description { get; set; }

			[JsonIgnore]
			public Password Password { get; set; }

			[JsonIgnore]
			public School School { get; set; }

			[JsonIgnore]
			public ICollection<UserSetting> UserSettings { get; set; }

			[JsonIgnore]
			public ICollection<Notification> ReadNotifications { get; set; }

			[JsonIgnore]
			public ICollection<AchievementUnlock> Achievements { get; set; }

			[JsonIgnore]
			public ICollection<Zolik> OriginalZoliks { get; set; }

			[JsonIgnore]
			public ICollection<UserLoginToken> LoginTokens { get; set; }

			[JsonIgnore]
			public ICollection<UserLogin> Logins { get; set; }

			[JsonIgnore]
			public ICollection<TeacherSubject> Teaching { get; set; }
		}
	}

	public partial class UserLog : IDbEntity { }

	public partial class UserLogin : IDbObject, IDbEntity
	{
		[JsonIgnore]
		public Projects Project => (Projects) this.ProjectID;
	}

	public partial class UserLoginToken : IDbObject { }

	public partial class UserSetting : IDbEntity, IUserSetting
	{
		public UserSetting() { }
	}

	[MetadataType(typeof(ZolikMetadata))]
	public partial class Zolik : IDbObject, IDbEntity, IZolik
	{
		public bool IsLocked => !string.IsNullOrWhiteSpace(this.Lock);

		public bool CanBeTransfered => this.Enabled && !this.IsLocked && this.Type != ZolikType.Black;

		public bool IsSplittable => this.Enabled && this.AllowSplit && this.Type.IsSplittable();

		private sealed class ZolikMetadata
		{
			[JsonIgnore]
			public bool AllowSplit { get; set; }

			[JsonIgnore]
			public User OriginalOwner { get; set; }

			[JsonIgnore]
			public Subject Subject { get; set; }

			[JsonIgnore]
			public ICollection<Transaction> Transactions { get; set; }
		}
	}
}