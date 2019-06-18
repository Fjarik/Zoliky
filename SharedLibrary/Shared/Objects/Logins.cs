using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using SharedLibrary.Enums;

namespace SharedLibrary
{
	[Android.Runtime.Preserve(AllMembers = true)]
	public class Logins
	{
		public string UName { get; set; }
		public string Password { get; set; }
		public Projects Project { get; set; }
		public string Ver { get; set; } = "1.0.1";

		public Version Version
		{
			get => Version.Parse(Ver);
			set => Ver = value.ToString();
		}

		public Logins()
		{
			if (this.Project == 0) {
				Project = Projects.Unknown;
			}
		}

		public Logins([NotNull] string uname, [NotNull] string pwd, Projects project) : this()
		{
			UName = uname;
			Password = pwd;
			Project = project;
		}

		public Logins([NotNull] string uname, [NotNull] string pwd, Projects project, [NotNull] Version version) :
			this(uname, pwd, project)
		{
			Version = version;
		}
	}
}