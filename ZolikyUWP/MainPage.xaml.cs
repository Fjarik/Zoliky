﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Xml;
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
using SharedApi.Connectors;
using Microsoft.Toolkit.Uwp.Notifications;
using ZolikyUWP.Account;
using XmlDocument = Windows.Data.Xml.Dom.XmlDocument;

namespace ZolikyUWP
{
	public sealed partial class MainPage : Page
	{
		private User _me;

		public MainPage()
		{
			this.InitializeComponent();

			string appName = Windows.ApplicationModel.Package.Current.DisplayName;
			//AppTitle.Text = appName;
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
			RefreshNotification(_me.ID);
		}

		private void NvSample_OnLoaded(object sender, RoutedEventArgs e)
		{
			NavigateTo(typeof(DefaultPage));
		}

		private void NvSample_OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
		{
			if (!(args.SelectedItem is NavigationViewItem item)) {
				return;
			}

			if (item.Content != null) {
				NavMain.Header = item.Content;
			}

			if (item.Tag == null) {
				NavigateTo(typeof(DefaultPage));
				return;
			}

			switch (item.Tag.ToString()) {
				default:
					NavigateTo(typeof(DefaultPage));
					break;
			}
		}

		private void NavigateTo(Type pageType)
		{
			ContentFrame.Navigate(pageType, _me);
		}

		private void RefreshNotification(int id)
		{
			int count = _me.Zoliky.Count;

			var tile = Tiles.GetTileXml(count.ToString());


			var tileNot = new TileNotification(tile);
			TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNot);
			ScheduleNotification(id);
		}

		private void ScheduleNotification(int id)
		{
			var content = new Uri($"https://www.old.zoliky.eu/api/zolik/getuserzoliks/xml/{id}");
			var interval = PeriodicUpdateRecurrence.SixHours;

			var updater = TileUpdateManager.CreateTileUpdaterForApplication();
			updater.StartPeriodicUpdate(content, interval);
		}
	}

	public static class Tiles
	{
		public static XmlDocument GetTileXml(string count)
		{
			var tileContent = new TileContent() {
				Visual = new TileVisual() {
					TileMedium = new TileBinding() {
						Content = new TileBindingContentAdaptive() {
							Children = {
								new AdaptiveText() {
									Text = "Počet žolíků:",
									HintStyle = AdaptiveTextStyle.Caption,
									HintAlign = AdaptiveTextAlign.Left,
									HintMaxLines = 1
								},
								new AdaptiveText() {
									Text = count,
									HintStyle = AdaptiveTextStyle.Subheader,
									HintAlign = AdaptiveTextAlign.Center,
									HintMaxLines = 1
								},
								/*new AdaptiveText()
								{
									Text = name,
									HintStyle = AdaptiveTextStyle.CaptionSubtle,
									HintAlign = AdaptiveTextAlign.Left,
									HintMaxLines = 1
								}*/
							}
						},
						DisplayName = "Žolíky",
						Branding = TileBranding.NameAndLogo
					}
				}
			};
			return tileContent.GetXml();
		}
	}
}