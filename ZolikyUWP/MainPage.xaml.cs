using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SharedApi.Models;
using ZolikyUWP.Account;
using ZolikyUWP.Pages;
using ZolikyUWP.Tools;
using ZolikConnector = SharedApi.Connectors.New.ZolikConnector;

namespace ZolikyUWP
{
	public sealed partial class MainPage : Page
	{
		private User _me;

		private CommandBar NavCommandsBar => this.NavMain.FindControl<CommandBar>("NavCommands");

		private AppBarButton UpdateButton => this.NavCommandsBar.FindControl<AppBarButton>("UpdateButton");
		private AppBarButton PinButton => this.NavCommandsBar.FindControl<AppBarButton>("PinButton");

		private AppBarButton InfoButton =>
			this.NavCommandsBar.SecondaryCommands.FirstOrDefault(x => x is AppBarButton btn && btn.Name == "InfoButton")
				as AppBarButton;

		public MainPage()
		{
			this.InitializeComponent();
			this.DataContext = this;
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

			var api = new ZolikConnector(_me.Token);

			var count = await api.GetUserZolikCountAsync(_me.ID);
			RefreshNotification(_me.ID, count);

			var localSettings = ApplicationData.Current.LocalSettings;
			localSettings.Values[StorageKeys.LastZolikCount] = count;
			base.OnNavigatedTo(e);
		}

		private void NvSample_OnLoaded(object sender, RoutedEventArgs e)
		{
			NavigateTo(typeof(DefaultPage));
		}

		private void NvSample_OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
		{
			if (!(args.SelectedItem is NavigationViewItem item)) {
				return;
			}

			if (item.Content != null) {
				NavMain.Header = item.Content;
			}

			if (item.Tag == null) {
				NavigateTo(typeof(DefaultPage));
				return;
			}

			var type = typeof(DefaultPage);
			switch (item.Tag.ToString()) {
				case "Zolici":
					type = typeof(ZoliciPage);
					break;
				case "Transactions":
					type = typeof(TransactionPage);
					break;
				case "Achievements":
					type = typeof(AchievementsPage);
					break;
			}

			NavigateTo(type);
		}

		private void UpdateButton_OnClick(object sender, RoutedEventArgs e)
		{
			if (this.ContentFrame.Content is IUpdatable page) {
				page.UpdateAsync();
			}
		}

		private async void BtnPin_OnClick(object sender, RoutedEventArgs e)
		{
			await InitOrDeleteTile();
		}

		private void NavigateTo(Type pageType)
		{
			var btns = new[] {UpdateButton, InfoButton};
			foreach (var btn in btns) {
				if (btn != null) {
					btn.Visibility = pageType.GetInterfaces().Contains(typeof(IUpdatable))
										 ? Visibility.Visible
										 : Visibility.Collapsed;
				}
			}

			ContentFrame.Navigate(pageType, _me);
		}

		private async void BtnInfo_OnClick(object sender, RoutedEventArgs e)
		{
			if (this.ContentFrame.Content is IUpdatable page) {
				var dialog = new ContentDialog {
					Title = "Info",
					Content = "Poslední aktualizace: " + Environment.NewLine +
							  page.LastUpdate,
					PrimaryButtonText = "Ok",
				};
				await dialog.ShowAsync();
			}
		}

		private void BtnLogout_OnClick(object sender, RoutedEventArgs e)
		{
			var localSettings = ApplicationData.Current.LocalSettings;
			localSettings.Values.Remove(StorageKeys.LastToken);
			localSettings.Values.Remove(StorageKeys.LastZolikCount);
			this.Frame.Navigate(typeof(LoginPage));
		}

#region Tiles

#region InitOrDelete

		private async Task InitOrDeleteTile()
		{
			var id = StorageKeys.TileId;
			if (SecondaryTile.Exists(id)) {
				await DeleteTile(id);
				this.CheckIfPinned(id);
				return;
			}
			var localSettings = ApplicationData.Current.LocalSettings;
			if (localSettings.Values[StorageKeys.LastZolikCount] is int count) {
				await InitTile(_me.ID, count, id);
				this.CheckIfPinned(id);
			}
		}

		private async Task InitTile(int userId, int count, string id)
		{
			if (SecondaryTile.Exists(id)) {
				return;
			}
			var tile = new SecondaryTile(id,
										 "Žolíky",
										 "action=zoliky",
										 new Uri("ms-appx:///Assets/LargeTile.scale-150.png"),
										 TileSize.Default);
			var isPinned = await tile.RequestCreateAsync();
			if (!isPinned) {
				return;
			}
			RefreshNotification(userId, count, id);
		}

		private async Task DeleteTile(string id)
		{
			var toDelete = new SecondaryTile(id);
			await toDelete.RequestDeleteAsync();
		}

#endregion

#region Notifications

		private void RefreshNotification(int userId, int count, string tileId = StorageKeys.TileId)
		{
			if (!SecondaryTile.Exists(tileId)) {
				return;
			}

			var tile = Tiles.GetTileXml(count.ToString());
			var tileNot = new TileNotification(tile);

			var updater = TileUpdateManager.CreateTileUpdaterForSecondaryTile(tileId);
			updater.Update(tileNot);

			ScheduleNotification(userId, updater);
			this.CheckIfPinned(tileId);
		}

		private void ScheduleNotification(int id, TileUpdater updater)
		{
			var content =
				new Uri($"https://www.api.zoliky.eu/public/userZoliksXml?userId={id}&password=yCpJZrc18Dn5JSxE");
			var interval = PeriodicUpdateRecurrence.SixHours;

			updater.StartPeriodicUpdate(content, interval);
		}

#endregion

		private void CheckIfPinned(string tileId)
		{
			var btn = PinButton;
			if (btn == null) {
				return;
			}
			if (SecondaryTile.Exists(tileId)) {
				btn.Label = "Odepnout";
				btn.Icon = new SymbolIcon(Symbol.UnPin);
			} else {
				btn.Label = "Připnout";
				btn.Icon = new SymbolIcon(Symbol.Pin);
			}
		}

#endregion
	}
}