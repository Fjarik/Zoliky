using Flurl.Http;
using SharedApi.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SharedLibrary.Enums;
using SharedLibrary;
using Crash = SharedApi.Models.Crash;

namespace SharedApi.Connectors
{
	public sealed class CrashConnector : ApiConnector
	{
		public CrashConnector() : base() { }

		public CrashConnector([NotNull]string token) : base(token) { }

		public MActionResult<Crash> Create(CrashPackage pack)
		{
			if (pack == null) {
				return new MActionResult<Crash>(StatusCode.InvalidInput);
			}
			try {
				return Task.Run(() => Request("crash/create", UsedToken).PostJsonAsync(pack).ReceiveJson<MActionResult<Crash>>()).Result;
			} catch (Exception ex) {
				return new MActionResult<Crash>(StatusCode.SeeException, ex);
			}
		}

	}
}
