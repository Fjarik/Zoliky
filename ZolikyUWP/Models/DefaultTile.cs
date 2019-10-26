using System;
using JetBrains.Annotations;
using ZolikyUWP.Pages;

namespace ZolikyUWP.Models
{
	public class DefaultTile
	{
		[NotNull]
		public string Title { get; set; } = "Test";

		[NotNull]
		public string Content { get; set; } = "";

		[NotNull]
		public Type RedirectPageType { get; set; } = typeof(DefaultPage);

		[CanBeNull]
		public object RedirectArgs { get; set; } = null;
	}
}