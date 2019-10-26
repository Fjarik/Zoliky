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
		public List<DefaultTile> TilesItems { get; set; }

		public DefaultPage()
		{
			this.InitializeComponent();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			if (e.Parameter == null) {
				this.Frame.Navigate(typeof(LoginPage));
				return;
			}
			if (e.Parameter is User u) {
				_me = u;
			}

			Lbl_Name.Text = _me.FullName;
			var localSettings = ApplicationData.Current.LocalSettings;
			var count = localSettings.Values[StorageKeys.LastZolikCount];

			Lbl_Count.Text = count.ToString();

			/*
			TilesItems = new List<DefaultTile>()
			{
				new DefaultTile()
				{
					Title = "Test"
				},
				new DefaultTile()
				{
					Title = "Ahoj"
				},
				new DefaultTile()
				{
					Title = "Zdravím"
				},
				new DefaultTile()
				{
					Title = "Jak"
				},
			};*/

			this.DataContext = this;
		}

		private void Tiles_OnItemClick(object sender, ItemClickEventArgs e) { }
	}
}