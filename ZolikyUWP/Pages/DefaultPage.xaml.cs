using System.Collections.Generic;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SharedApi.Models;
using ZolikyUWP.Account;
using ZolikyUWP.Models;
using ZolikyUWP.Tools;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ZolikyUWP.Pages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class DefaultPage : Page
	{
		private User _me;

		public DefaultPage()
		{
			this.InitializeComponent();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			if (e.Parameter == null) {
				this.Frame.Navigate(typeof(LoginPage));
				return;
			}
			if (e.Parameter is User u) {
				_me = u;
			}

			var name = _me.FullName;
			if (!string.IsNullOrEmpty(_me.ClassName)) {
				name += $", {_me.ClassName}";
			}
			LblName.Text = name;

			if (!string.IsNullOrEmpty(_me.SchoolName)) {
				LblSchool.Text = _me.SchoolName;
			}

			LblLastLogin.Text = _me.LastLoginDate?.ToString("dd.MM.yyyy HH:mm") ?? "První přihlášení";

			base.OnNavigatedTo(e);
		}
	}
}