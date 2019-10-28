using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MahApps.Metro.IconPacks;
using Microsoft.Toolkit.Uwp.UI.Controls;
using SharedApi.Connectors.New;
using SharedApi.Models;
using ZolikyUWP.Account;
using ZolikyUWP.Dialogs;
using ZolikyUWP.Tools;

namespace ZolikyUWP.Pages
{
	public sealed partial class ZoliciPage : Page, IUpdatable
	{
		private User _me;
		public List<Zolik> Zoliks { get; set; }

		public bool IsLoading { get; set; }
		public DateTime LastUpdate { get; set; }

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

		public ZoliciPage()
		{
			this.InitializeComponent();
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			if (e.Parameter == null) {
				this.Frame.Navigate(typeof(LoginPage));
				return;
			}
			if (e.Parameter is User u) {
				_me = u;
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

		private void ZoliksGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!(ZoliksGrid.SelectedItem is Zolik selected)) {
				return;
			}

			foreach (var zolik in Zoliks.Where(zolik => zolik.ID != selected.ID)) {
				if (ZoliksGrid.Columns[0].GetCellContent(zolik) is FontIcon original) {
					original.Glyph = "\uE76C";
				}
			}

			if (ZoliksGrid.Columns[0].GetCellContent(selected) is FontIcon icon) {
				icon.Glyph = "\uE70D";
			}
		}

		private async void BtnZolikAction_OnClick(object sender, RoutedEventArgs e)
		{
			if (!(sender is Button b) || b.Tag == null) {
				return;
			}
			if (!(ZoliksGrid.SelectedItem is Zolik selected)) {
				return;
			}
			var id = selected.ID;

			var tag = b.Tag.ToString();
			bool success;
			var showDialog = true;
			SetLoading(true);
			switch (tag) {
				case "Lock":
					success = await LockAsync(id);
					showDialog = false;
					break;
				case "Unlock":
					success = await UnlockAsync(id);
					showDialog = false;
					break;
				case "Give":
					success = await GiveAsync(id);
					break;
				case "Split":
					success = await SplitAsync(id);
					break;
				default:
					SetLoading(false);
					return;
			}
			if (showDialog && success) {
				var dialog = new ContentDialog {
					Title = "Otevřeno okno přohlížeče",
					Content =
						"Přejděte do svého internetového přohlížeče, kde se Vám otevřela stránka s Vámi vyžadovanou funkcionalitou. " +
						"Po dokončení klikněte na tlačítko níže." + Environment.NewLine + Environment.NewLine +
						"Poznámka: Nejspíše se budete muset přihlásit a následně budete přesměrováni.",
					CloseButtonText = "Hotovo"
				};
				await dialog.ShowAsync();
			}
			await UpdateAsync();
		}

		private async Task<bool> LockAsync(int zolikId)
		{
			var api = new ZolikConnector(_me.Token);

			var dialog = new LockDialog();
			var dialogRes = await dialog.ShowAsync();
			if (dialogRes != ContentDialogResult.Primary || string.IsNullOrEmpty(dialog.Text)) {
				return false;
			}
			var text = dialog.Text;
			var res = await api.LockZolikAsync(zolikId, text);
			if (res.IsSuccess) {
				return true;
			}
			var err = res.GetStatusMessage();
			var a = await this.ShowErrorDialogAsync(err);
			return false;
		}

		private async Task<bool> UnlockAsync(int zolikId)
		{
			var api = new ZolikConnector(_me.Token);

			var res = await api.UnlockZolikAsync(zolikId);
			if (res.IsSuccess) {
				return true;
			}
			var err = res.GetStatusMessage();
			var a = await this.ShowErrorDialogAsync(err);
			return false;
		}

		private async Task<bool> GiveAsync(int zolikId)
		{
			var url = $"https://www.zoliky.eu/App/Zoliky/Transfer?id={zolikId}";

			var uri = new Uri(url);

			var success = await Windows.System.Launcher.LaunchUriAsync(uri);
			return success;
		}

		private async Task<bool> SplitAsync(int zolikId)
		{
			var url = $"https://www.zoliky.eu/App/Zoliky/Split?id={zolikId}";

			var uri = new Uri(url);

			var success = await Windows.System.Launcher.LaunchUriAsync(uri);
			return success;
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

			var api = new ZolikConnector(_me.Token);
			Zoliks = await api.GetUserZoliksAsync(_me.ID);
			ZoliksGrid.ItemsSource = Zoliks;

			var localSettings = ApplicationData.Current.LocalSettings;
			localSettings.Values[StorageKeys.LastZolikCount] = Zoliks.Count;
			this.LastUpdate = DateTime.Now;
			await Task.Delay(500);

			this.IsLoading = false;
			SetLoading(false);
		}
	}
}