using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.UI.Controls;
using SharedApi.Connectors.New;
using SharedApi.Models;
using SharedLibrary;
using SharedLibrary.Shared.Objects;
using ZolikyUWP.Account;
using ZolikyUWP.Tools;

namespace ZolikyUWP.Pages
{
	public sealed partial class LoginHistoryPage : Page, IUpdatable
	{
		private User _me;

		public bool IsLoading { get; set; }
		public DateTime LastUpdate { get; set; }
		public List<UserLogin> UserLogins { get; set; }

		private Loading LoadingElement
		{
			get
			{
				if (!(this.Frame.Parent is NavigationView nav)) {
					return null;
				}
				if (!(nav.Parent is Grid grid)) {
					return null;
				}
				if (!(grid.FindName("LoadingControl") is Loading l)) {
					return null;
				}
				return l;
			}
		}

		public LoginHistoryPage()
		{
			this.InitializeComponent();
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			switch (e.Parameter) {
				case null:
					this.Frame.Navigate(typeof(LoginPage));
					return;
				case User u:
					_me = u;
					break;
			}
			await UpdateAsync();
			base.OnNavigatedTo(e);
		}

		private void SetLoading(bool loading)
		{
			if (LoadingElement != null) {
				this.LoadingElement.IsLoading = loading;
			}
		}

		private async Task<ContentDialogResult> ShowErrorDialogAsync(string error)
		{
			var dialog = new ContentDialog {
				Title = "Vyskytla se chyba",
				Content = error,
				CloseButtonText = "Ok"
			};
			var res = await dialog.ShowAsync();
			return res;
		}

		public async Task UpdateAsync()
		{
			if (IsLoading) {
				return;
			}
			this.IsLoading = true;
			SetLoading(true);

			var api = new UserConnector(_me.Token);
			var res = await api.GetUserLogins();
			if (!res.IsSuccess) {
				await ShowErrorDialogAsync(res.GetStatusMessage());
				this.IsLoading = false;
				return;
			}
			UserLogins = res.Content;
			LoginsGrid.ItemsSource = UserLogins;

			this.LastUpdate = DateTime.Now;
			await Task.Delay(500);

			this.IsLoading = false;
			SetLoading(false);
		}
	}
}