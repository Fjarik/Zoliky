using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Flurl.Http;
using SharedApi.Models;
using SharedLibrary.Enums;
using SharedLibrary.Shared.Objects;

namespace SharedApi.Connectors.New
{
	public class AchievementConnector : ApiConnector<Achievement>
	{
		public AchievementConnector(string token, ApiUrl url = ApiUrl.Zoliky) : base(token, url) { }

		public async Task<List<AchievementModel>> GetUserAchivementsAsync()
		{
			var url = "achievement/getuserachivements";
			try {
				var a = await Request(url).GetJsonAsync<List<AchievementModel>>();
				return a;
			} catch (Exception ex) {
				return new List<AchievementModel>();
			}
		}
	}
}