using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Microsoft.Toolkit.Uwp.Notifications;

namespace ZolikyUWP.Tools
{
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