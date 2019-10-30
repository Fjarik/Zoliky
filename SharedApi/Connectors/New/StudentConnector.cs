using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Flurl.Http;
using SharedApi.Models;
using SharedLibrary;
using SharedLibrary.Enums;
using SharedLibrary.Shared.ApiModels;

namespace SharedApi.Connectors.New
{
	public class StudentConnector : ApiConnector<Student>
	{
		public StudentConnector(string token, ApiUrl url = ApiUrl.Zoliky) : base(token, url) { }

		public async Task<List<Student>> GetStudentsAsync(int? classId = null, int? imageMaxSize = null)
		{
			var url = "student/getstudents";
			if (classId != null) {
				url += $"?classId={classId}";
				if (imageMaxSize != null && imageMaxSize > 0) {
					url += $"&imageMaxSize={imageMaxSize}";
				}
			} else {
				if (imageMaxSize != null && imageMaxSize > 0) {
					url += $"?imageMaxSize={imageMaxSize}";
				}
			}
			try {
				var res = await Request(url).GetJsonAsync<List<Student>>();
				return res;
			} catch {
#if (DEBUG)
				throw;
#endif
				return new List<Student>();
			}
		}

		public async Task<List<Student>> GetFakeStudentsAsync()
		{
			try {
				var res = await Request("student/getfake").GetJsonAsync<List<Student>>();
				return res;
			} catch {
#if (DEBUG)
				throw;
#endif
				return new List<Student>();
			}
		}

		public async Task<List<Student<Image>>> GetStudentsWithMostZoliks(int? classId = null)
		{
			try {
				var url = "student/gettop";
				if (classId != null) {
					url += $"?classId={classId}";
				}
				var res = await Request(url).GetJsonAsync<List<Student<Image>>>();
				return res;
			} catch {
#if (DEBUG)
				throw;
#endif
				return new List<Student<Image>>();
			}
		}
	}
}