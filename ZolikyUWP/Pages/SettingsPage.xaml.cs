using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Newtonsoft.Json;
using SharedApi.Models;
using SharedLibrary.Enums;
using ZolikyUWP.Account;
using ZolikyUWP.Tools;

namespace ZolikyUWP.Pages
{
	public sealed partial class SettingsPage : Page, IUpdatable
	{
		private User _me;

		public CustomUser CurrentUser { get; set; }
		public bool IsLoading { get; set; }
		public DateTime LastUpdate { get; set; }
		public int CurrentYear => DateTime.Today.Year;
		public string CurrentVersion => Extensions.GetAppVersion();

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

		public SettingsPage()
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

		private void SettingsPage_OnLoaded(object sender, RoutedEventArgs e)
		{
			this.DataContext = this;
			GridAccount.DataContext = CurrentUser;
			LoadTheme(AppSettings.Theme);
		}

		private void SetLoading(bool loading)
		{
			if (LoadingElement != null) {
				this.LoadingElement.IsLoading = loading;
			}
		}

		public async Task UpdateAsync()
		{
			if (IsLoading) {
				return;
			}
			this.IsLoading = true;
			SetLoading(true);

			var json = JsonConvert.SerializeObject(_me);
			CurrentUser = JsonConvert.DeserializeObject<CustomUser>(json);
			CurrentUser.EncodedImage = CurrentUser.ProfilePhoto?.Base64;

			/*
			var api = new TransactionConnector(_me.Token);
			var tRes = await api.GetUserTransactions(_me.ID, 50);
			if (!tRes.IsSuccess) {
				await ShowErrorDialogAsync(tRes.GetStatusMessage());
				this.IsLoading = false;
				return;
			}
			Transactions = tRes.Content;
			TransactionsGrid.ItemsSource = Transactions;*/

			this.LastUpdate = DateTime.Now;
			await Task.Delay(500);

			this.IsLoading = false;
			SetLoading(false);
		}

		private void ToggleTheme_OnChecked(object sender, RoutedEventArgs e)
		{
			if (sender is RadioButton b && !string.IsNullOrEmpty(b.Tag?.ToString())) {
				ChangeTheme(b.Tag.ToString());
			}
		}

		private void ChangeTheme(string theme)
		{
			if (string.IsNullOrEmpty(theme)) {
				return;
			}
			// Default
			var set = ElementTheme.Dark;
			switch (theme) {
				case "Light":
					set = ElementTheme.Light;
					break;
				case "Dark":
					break;
				default:
					var defaultTheme = new Windows.UI.ViewManagement.UISettings();
					var uiTheme = defaultTheme.GetColorValue(Windows.UI.ViewManagement.UIColorType.Background)
											  .ToString();
					if (uiTheme == "#FFFFFFFF") {
						set = ElementTheme.Light;
					}
					break;
			}
			this.RequestedTheme = set;
			AppSettings.Theme = set;
		}

		private void LoadTheme(ElementTheme theme)
		{
			RadiBtnSystem.IsChecked = false;
			RadiBtnLight.IsChecked = false;
			RadiBtnDark.IsChecked = false;
			switch (theme) {
				case ElementTheme.Default:
					RadiBtnSystem.IsChecked = true;
					break;
				case ElementTheme.Light:
					RadiBtnLight.IsChecked = true;
					break;
				case ElementTheme.Dark:
					RadiBtnDark.IsChecked = true;
					break;
			}
		}
	}

	public class CustomUser : User, INotifyPropertyChanged
	{
		public string GenderString => base.Sex.GetDescription();

		private string _encodedImage;

		public string EncodedImage
		{
			get => _encodedImage;
			set
			{
				_encodedImage = value;
				ConvertToImage(value);
				NotifyPropertyChanged("EncodedStream");
			}
		}

		private WriteableBitmap _selectedImage;

		public WriteableBitmap SelectedImage
		{
			get => _selectedImage;
			private set
			{
				_selectedImage = value;
				NotifyPropertyChanged("SelectedImage");
			}
		}

		private async void ConvertToImage(string base64String)
		{
			byte[] bytes = Convert.FromBase64String(base64String);

			InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream();
			await stream.WriteAsync(bytes.AsBuffer());
			stream.Seek(0);

			BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
			WriteableBitmap writebleBitmap = new WriteableBitmap((int) decoder.PixelWidth, (int) decoder.PixelHeight);
			await writebleBitmap.SetSourceAsync(stream);
			SelectedImage = writebleBitmap;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged(string propName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
		}
	}
}