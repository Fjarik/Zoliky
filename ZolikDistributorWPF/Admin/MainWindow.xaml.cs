using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using JetBrains.Annotations;
using MahApps.Metro.Controls;
using SharedLibrary.Enums;
using SharedLibrary.Interfaces;
using User = SharedApi.Models.User;

namespace ZolikDistributor.Admin
{
	public partial class MainWindow : MetroWindow
	{
		private readonly User _meReference;

		public MainWindow(User me)
		{
			InitializeComponent();
			_meReference = me;
			TxtWelcome.Text = WelcomeText();

#if !(DEBUG)
			BtnNews.Visibility = Visibility.Hidden;
#endif
		}

		private void MainWindow_OnInitialized(object sender, EventArgs e)
		{
			LoadStats();
		}

		public void LoadStats()
		{
			if (!this.IsInitialized) {
				return;
			}
			var z = Globals.ZolikOwnerIds;
			var total = z.Count;
			var usersWithZoliks = z.Distinct().Count();
			var totalUsers = Globals.StudentsCount; // await Task.Run(() => mgr.Users.GetStudentsCount());

			TxtZolikyTotal.Text = total.ToString();
			TxtUsersWithZolik.Text = $"{usersWithZoliks}/{totalUsers}";

			GridMain.Visibility = Visibility.Visible;
			GridLoading.Visibility = Visibility.Hidden;
		}

		public async Task RefreshStatsAsync()
		{
			if (this.IsInitialized) {
				GridLoading.Visibility = Visibility.Visible;
				GridMain.Visibility = Visibility.Hidden;
			}
			await Globals.RefreshZolikStatsAsync(_meReference.Token);
			await Task.Delay(300);
			LoadStats();
		}

		private string WelcomeText()
		{
			string nowS = DateTime.Now.ToString("HH");
			var hour = Convert.ToInt32(nowS);
			if (hour > 5 && hour < 12) {
				return $"Dobré ráno, {_meReference.Name}!";
			}
			if (hour > 11 && hour < 19) {
				return $"Dobré odpoledne, {_meReference.Name}!";
			}
			if (hour > 18) {
				return $"Dobrý večer, {_meReference.Name}!";
			}
			return $"Dobrý den, {_meReference.Name}!";
		}

		private void Btn_AssignZoliky_Click(object sender, RoutedEventArgs e)
		{
			var za = new ZolikyAsignment(_meReference) {
				Owner = this
			};
			za.ShowDialog();
			//RefreshStats();
		}

		private void Btn_RemoveZoliky_Click(object sender, RoutedEventArgs e)
		{
			var rem = new ZolikRemover(_meReference) {
				Owner = this
			};
			rem.ShowDialog();
			//RefreshStats();
		}

		private void Btn_logout_Click(object sender, RoutedEventArgs e)
		{
			var style = new Style();
			style.Setters.Add(new Setter(Xceed.Wpf.Toolkit.MessageBox.YesButtonContentProperty, "Ano"));
			style.Setters.Add(new Setter(Xceed.Wpf.Toolkit.MessageBox.NoButtonContentProperty, "Ne"));

			MessageBoxResult mbr =
				Xceed.Wpf.Toolkit.MessageBox
					 .Show("Přejete si při příštím spuštění aplikace, aby vyžadovala opětovné přihlášení?",
						   "Odhlášení", MessageBoxButton.YesNo, MessageBoxImage.None, MessageBoxResult.Yes, style);

			if (mbr == MessageBoxResult.Yes) {
				Properties.Settings.Default.rememberMe = false;
			}
			Properties.Settings.Default.Save();
			this.Close();
		}

		private async void BtnRefresh_OnClick(object sender, RoutedEventArgs e)
		{
			await RefreshStatsAsync();
		}

		private void BtnRemoveNew_OnClick(object sender, RoutedEventArgs e)
		{
			var rn = new RemoverNew(_meReference) {
				Owner = this
			};
			this.WindowState = WindowState.Minimized;
			rn.Show();
		}
	}
}