using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SharedApi.Models;
using ZolikyUWP.Account;
using ZolikyUWP.Models;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ZolikyUWP
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
			//Lbl_Count.Text = _me.Zoliky.Count.ToString();
			/*
			UserConnector uc = new UserConnector(_me.Token);
			var mAu = uc.GetTesterAccount(_me);
			if (mAu.IsSuccess) {
				_me = mAu.Content;
				bool b = _me.IsTesterType;
			}
			*/
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
					Title = "Ahojky"
				},
				new DefaultTile()
				{
					Title = "Jak"
				},
			};*/

			this.DataContext = this;
		}


		private void Tiles_OnItemClick(object sender, ItemClickEventArgs e)
		{
			

		}
	}
}
