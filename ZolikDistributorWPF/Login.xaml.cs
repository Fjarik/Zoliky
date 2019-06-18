using DataAccess.Models;
using MahApps.Metro.Controls;
using SharedLibrary;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using Newtonsoft.Json;
using Plugin.Connectivity;
using SharedApi.Connectors.New;
using SharedLibrary.Enums;
using SharedLibrary.Shared;
using ZolikDistributor.Admin;

namespace ZolikDistributor
{
	public partial class Login : MetroWindow
	{
		public Login()
		{
			InitializeComponent();

			StatusVersion.Content = Globals.Version.ToString();
			TxtInput.Text = Properties.Settings.Default.username;
			if (Properties.Settings.Default.rememberMe) {
				TxtPwd.Password = Properties.Settings.Default.password;
				ChBRemember.IsChecked = true;
				BtnLogin.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
			}
		}

		private async void BtnLogin_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(TxtInput.Text) || string.IsNullOrWhiteSpace(TxtPwd.Password)) {
				ShowMessage("Chyba", "Musíte zadat přihlašovací jméno a heslo");
				return;
			}
			ShowLoading("Testování připojení");

			var api = new PublicConnector();

#region Check connection

			if (!CrossConnectivity.Current.IsConnected) {
				ShowMessage("Chyba připojení",
							"Vaše zařízení není připojené k internetu.\nZkontrolujte připojení a zkuste to znovu.");
				return;
			}

			bool con = await api.CheckConnectionsAsync();
			if (!con) {
				ShowMessage("Chyba připojení",
							"Nepodařilo se připojit k serveru Žolíků. \nZkuste to prosím později.");
				return;
			}

#endregion

#region Check update

			var newestVersion = await api.GetWpfVersionAsync();
			if (!Globals.CompareVersions(newestVersion)) {
				var click = ShowMessage("Aktualizace", "Je dostupná aktualizace. Přejete si ji stáhnout?",
										MessageBoxButton.YesNo);
				if (click == MessageBoxResult.Yes) {
					ShowMessage("Aktualizace", "Nainstalujte soubor, který se Vám stáhne.");
					string url = $"http://skyzio.cz/Zoliky/Peta/{newestVersion}/ZolikySetup.exe";
					System.Diagnostics.Process.Start(url);
				} else {
					ShowMessage("Smůla", $"Aktualizace jsou povinné! {Environment.NewLine}Vypínám se.");
				}
				Application.Current.Shutdown();
				return;
			}

#endregion

#region Check database connection

			string title = "Server je nepřístupný";
			var ws = await api.CheckStatusAsync(Projects.WPF);
			if (ws == null) {
				ShowMessage(title,
							"Nepodařilo se zjistit stav serverů. \nZkuste to prosím později");
				return;
			}

#if !(DEBUG)
			if (!ws.CanAccess) {
				switch (ws.Status) {
					case PageStatus.Limited:
						title = "Omezený přístup";
						break;
					case PageStatus.NotAvailable:
						title = "Probíhá údržba";
						if (ws.Content != null && !string.IsNullOrWhiteSpace(ws.Content.ToString())) {
							var unv = JsonConvert.DeserializeObject<Unavailability>(ws.Content.ToString());
							if (unv != null) {
								ShowMessage(title,
											$"Důvod: {unv.Reason} {Environment.NewLine}Přepokládaný konec: {unv.To}");
								return;
							}
						}
						break;
				}

				if (string.IsNullOrWhiteSpace(ws.Message)) {
					ShowMessage(title,
								"Na severech aktuálně probíhá údržba a nelze se připojit. Zkuste to prosím později.");
					return;
				}

				ShowMessage(title, ws.Message);
				return;
			}
#endif

			try {
				using (ZoliksEntities ent = new ZoliksEntities()) {
					var conn = ent.Database.Connection;
					await conn.OpenAsync();
					conn.Close();
					conn.Dispose();
				}
			} catch {
				ShowMessage("Pepčovy internety detekovány",
							"Nelze se připojit k databázi. Jste na kabelu? \n" +
							"\n" +
							"\n" +
							"Pokud ano, bežte prosím na Wi-Fi. \n" +
							"\n" +
							"Pokud již jste na Wi-Fi, a přesto se nedaří připojit, tak to nahlašte Správci školní sítě. Za tuto chybu vývojáři nemohou!\n");
				return;
			}

#endregion

#region Login

			Logins lg = new Logins(TxtInput.Text, TxtPwd.Password, Projects.WPF);
			ShowLoading("Přihlašování");
			var res = await api.LoginAsync(lg);
			if (!res.IsSuccess) {
				TxtPwd.Password = "";
				TxtPwd.Focus();
			}
			switch (res.Status) {
				case StatusCode.SeeException:
					ShowMessage("Kritické selhání", $"{res.Exception}");
					return;
				case StatusCode.NotFound:
				case StatusCode.WrongPassword:
					ShowMessage("Nesprávné jméno nebo heslo",
								"Ať hledám 🔍,\njak hledám 🔎,\ntyto přihlašovací údaje nenajdu.");
					return;
				case StatusCode.NotEnabled:
					ShowMessage("Uživatel není aktivován",
								"Databáze vrací, že nejste enabled, tak budete asi disabled.");
					return;
				case StatusCode.InvalidInput:
					ShowMessage("Chyba", "Neplatné vstupní údaje");
					return;
				case StatusCode.OK:
					break;
				default:
					ShowMessage("Chyba", "Něco se nepovedlo.");
					return;
			}

#endregion

			var u = res.Content;

			if (!u.IsInRolesOr(UserRoles.Teacher, UserRoles.Administrator)) {
				ShowMessage("Nejste učitel", "K přihlášení nemáte dostatečné oprávnění!");
				return;
			}

			ShowLoading("Nastavování aplikace");
			var suc = await Globals.InitializeAsync(u.Token);
			if (!suc) {
				ShowMessage("Chyba", "Nezdařilo se načíst nastavení");
				return;
			}
			ShowLoading("Spouštění aplikace");
			await Task.Delay(300);
			MainWindow mw = new MainWindow(u);
			mw.Show();
			Application.Current.MainWindow = mw;
			Properties.Settings.Default.username = TxtInput.Text;
			Properties.Settings.Default.password = TxtPwd.Password;
			Properties.Settings.Default.rememberMe = ChBRemember.IsChecked == true;
			Properties.Settings.Default.Save();
			Close();
		}

		private MessageBoxResult ShowMessage(string title,
											 string text,
											 MessageBoxButton btn = MessageBoxButton.OK)
		{
			GridLogin.Visibility = Visibility.Visible;
			GridLoading.Visibility = Visibility.Hidden;

			var res = Xceed.Wpf.Toolkit.MessageBox.Show(Application.Current.MainWindow, text, title, btn);
			return res;
		}

		private void ShowLoading(string text = "",
								 bool visibility = true)
		{
			UcLoading.Text = text;
			if (visibility) {
				GridLoading.Visibility = Visibility.Visible;
				GridLogin.Visibility = Visibility.Hidden;
			} else {
				GridLoading.Visibility = Visibility.Hidden;
				GridLogin.Visibility = Visibility.Visible;
			}
		}
	}
}