using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.Interfaces
{
	public interface IUser : IEmailable, IStudent
	{
		string Username { get; set; }
		string SchoolName { get; }
	}

	public interface IUser<TClass> : IUser where TClass : class, IClass
	{
		TClass Class { get; set; }
	}

	public interface IUser<TClass, TImage> : IUser<TClass> where TClass : class, IClass where TImage : class, IImage
	{
		TImage ProfilePhoto { get; set; }
	}

	public interface IUser<TClass, TImage, TRole> : IUser<TClass, TImage>
		where TClass : class, IClass where TImage : class, IImage where TRole : class, IRole
	{
		ICollection<TRole> Roles { get; set; }

		bool IsInRole(string role);
		bool IsInRolesOr(params string[] roles);
		bool IsInRolesAnd(params string[] roles);
	}
}