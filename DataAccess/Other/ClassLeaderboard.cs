using DataAccess.Models;
using SharedLibrary.Interfaces;

namespace DataAccess
{
	public class ClassLeaderboard : Class
	{
		public int ZolikCount { get; set; }

		public ClassLeaderboard() { }

		public ClassLeaderboard(IClass c, int zolikCount) : base(c)
		{
			this.ZolikCount = zolikCount;
		}
	}
}