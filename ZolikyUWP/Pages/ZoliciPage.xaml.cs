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
using Microsoft.Toolkit.Uwp.UI.Controls;
using SharedApi.Connectors.New;
using SharedApi.Models;
using ZolikyUWP.Account;

namespace ZolikyUWP.Pages
{
	public sealed partial class ZoliciPage : Page
	{
		private User _me;
		public List<Zolik> Zoliks { get; set; }

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
			SetLoading(true);
			var api = new ZolikConnector(_me.Token);
			Zoliks = await api.GetUserZoliksAsync(_me.ID);
			ZoliksGrid.ItemsSource = Zoliks;
			SetLoading(false);
			base.OnNavigatedTo(e);
		}

		private void SetLoading(bool loading)
		{
			if (LoadingElement != null) {
				this.LoadingElement.IsLoading = loading;
			}
		}
	}
}