using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace ZolikyUWP.Tools
{
	public static class Extensions
	{
		public static T FindControl<T>(this UIElement parent, string controlName) where T : FrameworkElement
		{
			if (parent == null)
				return null;

			if (parent.GetType() == typeof(T) && ((T) parent).Name == controlName) {
				return (T) parent;
			}
			T result = null;
			int count = VisualTreeHelper.GetChildrenCount(parent);
			for (int i = 0; i < count; i++) {
				var child = (UIElement) VisualTreeHelper.GetChild(parent, i);
				if (FindControl<T>(child, controlName) is T res) {
					result = res;
					break;
				}
			}
			return result;
		}

		public static string GetAppVersion()
		{
			Package package = Package.Current;
			PackageId packageId = package.Id;
			PackageVersion version = packageId.Version;

			return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
		}
	}
}