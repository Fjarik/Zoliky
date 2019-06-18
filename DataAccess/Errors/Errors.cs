using System.Data.Entity.Validation;
using System.Linq;

namespace DataAccess.Errors
{
	/*
	[Serializable]
	public class AlreadyExists : Exception
	{
		public AlreadyExists() { }

		public AlreadyExists(string message) : base(message) { }

		public AlreadyExists(string message, Exception innerException) : base(message, innerException) { }
	}

	[Serializable]
	public class NotValidID : Exception
	{
		public NotValidID() { }

		public NotValidID(string message) : base(message) { }

		public NotValidID(string message, Exception innerException) : base(message, innerException) { }
	}
	*/
	public static class DbExceptionExtend
	{
		public static string GetExceptionMessage(this DbEntityValidationException ex)
		{
			var errorMessages = ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage);
			var fullErrorMessage = string.Join("; ", errorMessages);
			var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
			return exceptionMessage;
		}
	}

}
