using System;
using System.Collections.Generic;
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
using SharedApi.Connectors.New;
using SharedApi.Models;
using SharedLibrary.Shared.ApiModels;
using ZolikyUWP.Account;
using ZolikyUWP.Tools;
using System.ComponentModel;
using Newtonsoft.Json;
using SharedLibrary.Interfaces;
using Image = SharedApi.Models.Image;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ZolikyUWP.Pages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class StatisticsPage : Page, IUpdatable
	{
		private User _me;
		public bool IsLoading { get; set; }
		public DateTime LastUpdate { get; set; }

		public List<TopStudentsModel> TopStudents { get; set; }
		public List<TopStudentsModel> TopSchoolStudents { get; set; }

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

		public StatisticsPage()
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


			var api = new StudentConnector(_me.Token);
			var top = await api.GetStudentsWithMostZoliks(_me.ClassID);
			var topSchool = await api.GetStudentsWithMostZoliks();

			var json = JsonConvert.SerializeObject(top);
			TopStudents = JsonConvert.DeserializeObject<List<TopStudentsModel>>(json);
			StudentsList.ItemsSource = TopStudents;
			TopStudents.ForEach(x => x.EncodedImage = x.Profile.Base64);

			json = JsonConvert.SerializeObject(topSchool);
			TopSchoolStudents = JsonConvert.DeserializeObject<List<TopStudentsModel>>(json);
			StudentsSchoolList.ItemsSource = TopSchoolStudents;
			TopSchoolStudents.ForEach(x => x.EncodedImage = x.Profile.Base64);


			this.LastUpdate = DateTime.Now;
			await Task.Delay(500);

			this.IsLoading = false;
			SetLoading(false);
		}
	}

	public class TopStudentsModel : Student<Image>, INotifyPropertyChanged
	{
		private string encodedImage;

		public string EncodedImage
		{
			get => encodedImage;
			set
			{
				encodedImage = value;
				// Convert source to writeblebitmap and set value to SelectedImage 
				ConvertToImage(value);
				NotifyPropertyChanged("EncodedStream");
			}
		}

		private WriteableBitmap selectedImage;

		public WriteableBitmap SelectedImage
		{
			get => selectedImage;
			private set
			{
				selectedImage = value;
				NotifyPropertyChanged("SelectedImage");
			}
		}

		/// <summary> 
		/// Convert encoded string to writeblebitmap 
		/// set result value to SelectedImage 
		/// </summary> 
		/// <param name="base64String"></param> 
		public async void ConvertToImage(string base64String)
		{
			byte[] bytes = Convert.FromBase64String(base64String);

			InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream();
			await stream.WriteAsync(bytes.AsBuffer());
			stream.Seek(0);

			BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
			WriteableBitmap writebleBitmap = new WriteableBitmap((int) decoder.PixelWidth, (int) decoder.PixelHeight);
			await writebleBitmap.SetSourceAsync(stream);
			// When SelectedImage value is changed, it will notify the front end 
			SelectedImage = writebleBitmap;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged(string propName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
		}
	}
}