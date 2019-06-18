using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using SharedApi.Connectors.New;
using SharedApi.Models;
using SharedLibrary.Interfaces;

namespace ZolikDistributor.Admin
{
	/// <summary>
	/// Interaction logic for ZolikTransactions.xaml
	/// </summary>
	public partial class ZolikTransactions : MetroWindow
	{
		private List<Transaction> Transactions { get; set; }
		private User Logged { get; set; }
		private readonly IZolik _zolik;

		public ZolikTransactions(User logged, IZolik zolik)
		{
			this.Logged = logged;
			this._zolik = zolik;
			DataContext = this;
			InitializeComponent();
		}

		private async void ZolikTransactions_OnInitialized(object sender, EventArgs e)
		{
			GridLoading.Visibility = Visibility.Visible;
			GridMain.Visibility = Visibility.Hidden;
			var tc = new TransactionConnector(Logged.Token);
			this.Transactions = await tc.GetZolikTransactions(_zolik.ID);
			this.DataGridTransactions.ItemsSource = Transactions;
			GridLoading.Visibility = Visibility.Hidden;
			GridMain.Visibility = Visibility.Visible;
		}

		private void DataGridTransactions_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			DataGridTransactions.UnselectAll();
			DataGridTransactions.UnselectAllCells();
		}
	}
}