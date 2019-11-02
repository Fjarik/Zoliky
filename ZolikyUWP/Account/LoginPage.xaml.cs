using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using Plugin.Connectivity;
using SharedApi.Connectors.New;
using SharedApi.Models;
using SharedLibrary;
using SharedLibrary.Enums;
using ZolikyUWP.Tools;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ZolikyUWP.Account
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class LoginPage : Page
	{
		public LoginPage()
		{
			this.InitializeComponent();
			GridName.Visibility = Visibility.Visible;
			GridPwd.Visibility = Visibility.Collapsed;
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			this.DataContext = this;
			var localSettings = ApplicationData.Current.LocalSettings;
			if (CrossConnectivity.Current.IsConnected) {
				EnableElements(false);
				var res = await InitByToken(localSettings);
				if (res) {
					base.OnNavigatedTo(e);
					return;
				}
				EnableElements(true);
			}

			localSettings = ApplicationData.Current.RoamingSettings;
			if (localSettings.Values.ContainsKey(StorageKeys.LastUsername)) {
				if (localSettings.Values[StorageKeys.LastUsername] is string username) {
					TxtLogin.Text = username;
					BtnContinue_OnClick(this, new RoutedEventArgs());
				}
			}

			base.OnNavigatedTo(e);
		}

		private async Task<bool> InitByToken(ApplicationDataContainer localSettings)
		{
			if (localSettings.Values.ContainsKey(StorageKeys.LastToken)) {
				if (localSettings.Values[StorageKeys.LastToken] is string token) {
					var res = await TryTokenAsync(token);
					return res;
				}
			}
			return false;
		}

		private async Task<bool> TryTokenAsync(string token)
		{
			var uc = new UserConnector(token);
			var res = await uc.GetMe();
			if (!res.IsSuccess) {
				return false;
			}
			var user = res.Content;
			user.Token = token;

			this.Frame.Navigate(typeof(MainPage), user);
			return true;
		}

		private async void BtnContinue_OnClick(object sender, RoutedEventArgs e)
		{
			MessageDialog dialog = new MessageDialog("", "Chyba připojení");
			dialog.Commands.Add(new UICommand("Ok") {Id = 0});
			dialog.DefaultCommandIndex = 0;

			if (string.IsNullOrWhiteSpace(TxtLogin.Text)) {
				dialog.Title = "Chyba";
				dialog.Content = "Musíte zadat přihlašovací jméno";
				await dialog.ShowAsync();
				return;
			}
			GridName.Visibility = Visibility.Collapsed;
			GridPwd.Visibility = Visibility.Visible;
			TxtPwd.Focus(FocusState.Programmatic);
		}

		private async void Btn_Login_Click(object sender, RoutedEventArgs e)
		{
			MessageDialog dialog = new MessageDialog("", "Chyba připojení");
			dialog.Commands.Add(new UICommand("Ok") {Id = 0});
			dialog.DefaultCommandIndex = 0;

			if (!CrossConnectivity.Current.IsConnected) {
				dialog.Content = "Vaše zařízení není připojené k internetu.Zkontrolujte připojení a zkuste to znovu.";
				await dialog.ShowAsync();
				return;
			}

			var api = new PublicConnector();
			bool con = await api.CheckConnectionsAsync();
			if (!con) {
				dialog.Content = "Nepodařilo se připojit k serveru Žolíků. Zkuste to prosím později.";
				await dialog.ShowAsync();
				return;
			}

			var ws = await api.CheckStatusAsync(Projects.UWP);
			if (ws == null) {
				dialog.Content = "Nepodařilo se zjistit stav serverů. Zkuste to prosím později";
				await dialog.ShowAsync();
				return;
			}
#if !(DEBUG)
			if (!ws.CanAccess) {
				string title = "Server je nepřístupný";

				switch (ws.Status) {
					case PageStatus.Limited:
						title = "Omezený přístup";
						break;
					case PageStatus.NotAvailable:
						title = "Probíhá údržba";
						if (ws.Content != null && !string.IsNullOrWhiteSpace(ws.Content.ToString())) {
							var unv = JsonConvert.DeserializeObject<Unavailability>(ws.Content.ToString());
							if (unv != null) {
								dialog.Title = title;
								dialog.Content =
									$"Důvod: {unv.Reason} {Environment.NewLine}Přepokládaný konec: {unv.To}";
								await dialog.ShowAsync();
								return;
							}
						}

						break;
				}
				dialog.Title = title;

				if (string.IsNullOrWhiteSpace(ws.Message)) {
					dialog.Content =
						"Na severech aktuálně probíhá údržba a nelze se připojit. Zkuste to prosím později.";
					await dialog.ShowAsync();
					return;
				}

				dialog.Content = ws.Message;
				await dialog.ShowAsync();
				return;
			}
#endif

			var login = TxtLogin.Text;

			if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(TxtPwd.Password)) {
				dialog.Title = "Chyba";
				dialog.Content = "Musíte zadat jméno a heslo";
				await dialog.ShowAsync();
				return;
			}
			login = login.Trim().ToLower();

			Logins lg = new Logins(login, TxtPwd.Password.Trim(), Projects.UWP);

			BtnLogin.Content = "Přihlašování";
			EnableElements(false);
			MActionResult<User> res;
			try {
				res = await api.LoginAsync(lg);
			} catch (Exception ex) {
				res = new MActionResult<User>(StatusCode.SeeException, ex);
			}
			await Task.Delay(500);
			string msg = res.GetStatusMessage();
			switch (res.Status) {
				case StatusCode.SeeException:
					msg = $"Vyskytla se následující chyba: {res.Exception}";
					break;
				case StatusCode.NotFound:
				case StatusCode.WrongPassword:
					msg = "Neplatné jméno nebo heslo";
					break;
				case StatusCode.OK:
					break;
				default:
					msg = "Vyskytla se nespecifikovaná chyba";
					break;
			}

			if (!res.IsSuccess) {
				TxtPwd.Password = "";
				await ShowErrorDialogAsync(msg);
				EnableElements(true);
				TxtLogin.Focus(FocusState.Programmatic);
				return;
			}

			var localSettings = ApplicationData.Current.LocalSettings;
			var u = res.Content;
			localSettings.Values[StorageKeys.LastToken] = u.Token;

			// Nastavení přenést mezi zařízeními
			localSettings = ApplicationData.Current.RoamingSettings;
			localSettings.Values[StorageKeys.LastUsername] = TxtLogin.Text;

			// LblResult.Text = $"{u.FullName}";
			this.Frame.Navigate(typeof(MainPage), u);
		}

		private async Task<ContentDialogResult> ShowErrorDialogAsync(string error)
		{
			var dialog = new ContentDialog {
				Title = "Neúspěšné přihlášení",
				Content = error,
				CloseButtonText = "Ok"
			};
			var res = await dialog.ShowAsync();
			return res;
		}

		private void EnableElements(bool isEnabled)
		{
			LoadingControl.IsLoading = !isEnabled;
			BtnLogin.IsEnabled = isEnabled;
			TxtLogin.IsEnabled = isEnabled;
			TxtPwd.IsEnabled = isEnabled;
		}

		private void TxtLogin_OnKeyUp(object sender, KeyRoutedEventArgs e)
		{
			if (e.Key == Windows.System.VirtualKey.Enter) {
				BtnContinue_OnClick(this, new RoutedEventArgs());
			}
		}

		private void Txt_Pwd_KeyUp(object sender, KeyRoutedEventArgs e)
		{
			if (e.Key == Windows.System.VirtualKey.Enter) {
				Btn_Login_Click(this, new RoutedEventArgs());
			}
		}

		private async void LinkRegister_Click(object sender, RoutedEventArgs e)
		{
			string url = @"https://www.zoliky.eu/Register";

			var uri = new Uri(url);

			bool success = await Windows.System.Launcher.LaunchUriAsync(uri);
			if (success) {
				// TODO: Something
			}
		}

		private void BtnBack_OnClick(object sender, RoutedEventArgs e)
		{
			TxtPwd.Password = "";
			GridPwd.Visibility = Visibility.Collapsed;
			GridName.Visibility = Visibility.Visible;
			TxtLogin.Focus(FocusState.Programmatic);
		}
	}
}