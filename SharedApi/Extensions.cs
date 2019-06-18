using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace SharedApi
{
	public static class Extensions
	{
		[CanBeNull]
		public static Models.Zolik Latest(this List<Models.Zolik> list)
		{
			if (list == null || list.Count < 1) {
				return null;
			}

			if (list.Count == 1) {
				return list.First();
			}

			return list.OrderByDescending(x => x.OwnerSince).FirstOrDefault();
		}

	}
}
