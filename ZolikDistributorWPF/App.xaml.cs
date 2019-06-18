using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Plugin.Connectivity;

namespace ZolikDistributor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                MessageBox.Show("Nelze se připojit k internetu!", "Katastrofální selhání", MessageBoxButton.YesNoCancel,MessageBoxImage.Stop);
                return;
            }

            Login lg = new Login();
            lg.Show();
            this.MainWindow = lg;
			/*
	        var nw = new ZolikDistributor.Admin.MainWinNew();
			nw.Show();*/


        }
    }
}
