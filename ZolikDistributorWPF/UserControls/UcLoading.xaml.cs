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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ZolikDistributor.UserControls
{
	/// <summary>
	/// Interaction logic for UcLoading.xaml
	/// </summary>
	public partial class UcLoading : UserControl
	{
		public string Text
		{
			get => LblLogging.Content.ToString();
			set => LblLogging.Content = value;
		}

		public Brush Fground
		{
			get => LblLogging.Foreground;
			set => LblLogging.Foreground = value;
		}

		public UcLoading()
		{
			InitializeComponent();
			LblLogging.Content = "Přihlašování...";
		}
	}
}