using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using SharedLibrary.Interfaces;

namespace SharedLibrary.Shared.ApiModels
{
	public class Student : IStudent
	{
		public int ID { get; set; }
		public int? ProfilePhotoID { get; set; }
		public int? ClassID { get; set; }
		public string Name { get; set; }
		public string Lastname { get; set; }
		public string ClassName { get; set; }
		public int ZolikCount { get; set; }
		public int XP { get; set; }

		[JsonIgnore]
		public string FullName => $"{Name} {Lastname}";

		public Student() { }

		public Student(IStudent from)
		{
			this.ID = from.ID;
			this.ProfilePhotoID = from.ProfilePhotoID;
			this.ClassID = from.ClassID;
			this.Name = from.Name;
			this.Lastname = from.Lastname;
			this.ClassName = from.ClassName;
		}
	}

	public class Student<T> : Student, IStudent<T> where T : class, IImage, new()
	{
		public T GetDefaultImage() => new T {
			ID = 4,
			OwnerID = 2,
			Base64 =
				"iVBORw0KGgoAAAANSUhEUgAAAMAAAADACAAAAAB3tzPbAAACOUlEQVR4Ae3aCQrrIBRG4e5/Tz+CBAlIkIAECUjoSt48z/GZeAvnrMCvc6/38XzxAAAAYC4AAAAAAAAAAAAAAAAAAAAAAAAAAAAMCAAAAAAAAAAAAAAAAPsagz4V4rq/FmCLTj/k4vYqgCN5/TKfjlcAJKff5pJ5QPH6Y77YBiz6a4thQJ30D03VKmB3+qfcbhOwO+l+waP/+VsEBgDV6USumgNMOtVkDbDoZIstQNHpiimA1+m8JUBSQ8kO4HBqyB1mAElNJTMAr6a8FcCmxjYjgKjGohGAU2POBmBXc7sJwKrmVhOAqOaiCUBQc8EEQO0JwPMB4ADASwhAe3yR8VPiP3/M8XOaPzQd/lLyp56xSuvnUGK0yHC313idCw6umNov+bhm5aK7fdWAZQ/WbdoXnlg5Y+mvfe2SxVdWj20FAAAAAAAAAAAAwFQAAJSS0hwmfVMIc0qlmAfsOQWvP+RDyrtNQM1L0D8WllxNAWqOXifzMVcbgG3xaswv22jAFp3a6zFteYw8fQ9DM6Amr275VG8GlFmdm8uNgDzpgqZ8EyB7XZTPNwDKpAubysWAOuvi5nolYHW6PLdeBjiCbikc1wCK0025cgUg68Zyf0DUrcXegKibi30Bq25v7QnYNKCtH+BwGpA7ugFmDWnuBSgaVOkECBpU6AOoGlbtAlg1rLULIGhYoQvAaViuC0AD6wE4Xh1QAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADA194CuqC6onikxXwAAAAASUVORK5CYII=",
			MIME = "image/png",
			Size = 626
		};

		public T ProfilePhoto { get; set; }

		public T Profile
		{
			get => ProfilePhoto ?? this.GetDefaultImage();
			set
			{
				if (value == null) {
					value = this.GetDefaultImage();
				}
				ProfilePhoto = value;
			}
		}

		public Student() { }

		public Student(IStudent<T> from) : base(from)
		{
			this.ProfilePhoto = from.ProfilePhoto;
		}
	}
}