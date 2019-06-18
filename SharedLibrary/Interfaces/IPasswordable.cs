namespace SharedLibrary.Interfaces
{
	public interface IPasswordable : IValidable
	{
		string Password { get; set; }
		string RepeatPassword { get; set; }

		void ClearPasswords();
	}
}