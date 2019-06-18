namespace SharedLibrary.Interfaces
{
	public interface IChangePassword : IPasswordable
	{
		string OldPassword { get; set; }
	}
}