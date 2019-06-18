using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Flurl.Http;
using Newtonsoft.Json;
using SharedApi.Models;
using SharedLibrary;
using SharedLibrary.Enums;

namespace SharedApi.Connectors.New
{
	public class UserConnector : ApiConnector<User>
	{
		public UserConnector(string token, ApiUrl url = ApiUrl.Zoliky) : base(token, url) { }

		
	}
}