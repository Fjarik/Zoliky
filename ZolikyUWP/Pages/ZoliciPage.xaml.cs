using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SharedApi.Connectors.New;
using SharedApi.Models;
using ZolikyUWP.Account;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ZolikyUWP.Pages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class ZoliciPage : Page
	{
		private User _me;
		public List<Zolik> Zoliks { get; set; }

		public ZoliciPage()
		{
			this.InitializeComponent();
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			if (e.Parameter == null) {
				this.Frame.Navigate(typeof(LoginPage));
				return;
			}
			if (e.Parameter is User u) {
				_me = u;
			}


			var api = new ZolikConnector(_me.Token);
			Zoliks = await api.GetUserZoliksAsync(_me.ID);
			//this.DataContext = this;
			ZoliksGrid.ItemsSource = Zoliks;
			base.OnNavigatedTo(e);
		}
	}
}