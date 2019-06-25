using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedApi.Connectors.New;
using SharedLibrary.Enums;
using SharedLibrary.Shared.ApiModels;
using Class = SharedApi.Models.Class;
using Subject = SharedApi.Models.Subject;

namespace ZolikDistributor
{
	internal static class Globals
	{
		// Project settings

#region Project settings

		public static Version Version => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

#endregion

#region Init

#region Static models

		public static List<Class> Classes { get; private set; } = new List<Class>();
		public static List<Student> Students { get; private set; } = new List<Student>();
		public static List<Subject> Subjects { get; private set; } = new List<Subject>();
		public static List<int> ZolikOwnerIds { get; private set; } = new List<int>();
		public static int StudentsCount { get; private set; } = 0;
		public static int FakeStudentsCount { get; private set; } = 0;

#endregion

		public static async Task<bool> InitializeAsync(string token)
		{
			try {
				var c = new ClassConnector(token);
				var s = new SubjectConnector(token);
				var student = new StudentConnector(token);

				Classes = await c.GetAllAsync();
				Classes.Add(new Class() {
					Enabled = true,
					ID = 0,
					Name = "Fake",
					Graduation = DateTime.MaxValue,
					Since = DateTime.MinValue
				});
				Students = await student.GetStudentsAsync(imageMaxSize: 1);
				StudentsCount = Students.Count;
				var fake = await student.GetFakeStudentsAsync();
				fake.ForEach(x => {
					x.ClassName = "Fake";
					x.ClassID = 0;
				});
				FakeStudentsCount = fake.Count;
				Students.AddRange(fake);
				Subjects = await s.GetAllAsync();

				await RefreshZolikStatsAsync(token);

				return true;
			} catch {
#if (DEBUG)
				throw;
#endif
				return false;
			}
		}

		public static async Task<bool> RefreshStudentsAsync(string token, int classId)
		{
			try {
				if (classId < 1) {
					return false;
				}
				var student = new StudentConnector(token);

				var s = await student.GetStudentsAsync(classId, imageMaxSize: 1);
				Students = Students.Where(x => x.ClassID != classId).ToList();
				Students.AddRange(s);
				StudentsCount = Students.Count;
				return true;
			} catch {
#if (DEBUG)
				throw;
#endif
				return false;
			}
		}

		public static void ZolikCountMinus(int studentId)
		{
			Students = Students.Select(x => {
								   if (x.ID == studentId) {
									   x.ZolikCount--;
								   }
								   return x;
							   })
							   .ToList();
		}

		public static async Task RefreshZolikStatsAsync(string token)
		{
			var zc = new ZolikConnector(token);
			ZolikOwnerIds = await zc.GetZolikOwnerIdsAsync();
		}

#endregion

#region Version compare

		public static bool CompareVersions(Version v)
		{
			if (v == null) {
				return false;
			}

#if DEBUG
			if (Version > v) {
				return true;
			}
#endif
			return v == Version;
		}

#endregion
	}
}