using System;

namespace SharedLibrary.Shared
{
	public class Names
	{
		public override string ToString()
		{
			throw new NotSupportedException();
		}
	}
	
	public class ApplicationNames : Names
	{
		public string Sessions => "sessions";
	}

	public class CookieNames : Names
	{
		public string Main => "main";
		public string FirstVisit => "first_visit";
		public string CookiesAccept => "accept_cookies";
		public string IsMobile => "is_mobile";
		public string Username => "uname";
		public string ShowWarningPopUp => "warningPopUps";

	}

	public class SessionNames : Names
	{
		public string TestKey => "SomeKey";
		public string LoggedUser => "Logged";
		public string ToLogin => "ToLogin";
		public string LastPage => "LastPg";
		public string LastBackgroundImage => "LastBack";
		public string RegisterUser => "regUser";
		public string ResetUser => "resetPwd";
		public string Manager => "manager";
		public string SchoolID => "schoolId";
		public string ImportedStudents => "importedStudents";

	}

	public class QueryStringNames : Names
	{
		public string Message => "msg";
		public string Redirect => "redir";
		public string Nickname => "nick";
		public string TransferType => "transfer";
		public string Activate => "activate";
		public string Type => "type";
		public string Active => "Active";
		public string ID => "id";
		public string Tester => "tester";
		public string Token => "token";
	}

	public class QueryMessages : Names
	{
		public string NotLogged => "notLogged";
		public string InsufficientPermissions => "InsufficientPermissions";
		public string NotImplemented => "notImplementedYet";
		public string Activated => "activated";
		public string EmailError => "emailError";
		public string Changed => "changed";

	}

}