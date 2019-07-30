using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.Interfaces
{
	public interface ISettings : IDbObject
	{
		int? ProjectID { get; set; }
		string Key { get; set; }
		string Value { get; set; }
		DateTime Changed { get; set; }
	}

	public interface IUserSetting : ISettings
	{
		int UserID { get; set; }
	}
}