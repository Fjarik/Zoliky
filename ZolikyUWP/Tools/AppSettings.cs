using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;

namespace ZolikyUWP.Tools
{
	public class AppSettings
	{
		private static ApplicationTheme DefaultAppTheme = ApplicationTheme.Dark;

		private static ApplicationDataContainer Settings = ApplicationData.Current.LocalSettings;

		public static ElementTheme Theme
		{
			get
			{
				if (Settings.Values[StorageKeys.LastTheme] is int lastTheme) {
					return (ElementTheme) lastTheme;
				}
				return ElementTheme.Default;
			}
			set => Settings.Values[StorageKeys.LastTheme] = (int) value;
		}

		public static ApplicationTheme AppTheme
		{
			get
			{
				switch (Theme) {
					case ElementTheme.Default:
						return DefaultAppTheme;
					case ElementTheme.Light:
						return ApplicationTheme.Light;
					default:
						return ApplicationTheme.Dark;
				}
			}
			set
			{
				if (value == ApplicationTheme.Dark) {
					Theme = ElementTheme.Dark;
				}
				Theme = ElementTheme.Light;
			}
		}
	}
}