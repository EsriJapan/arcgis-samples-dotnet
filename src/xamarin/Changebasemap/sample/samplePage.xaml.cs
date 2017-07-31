using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Ogc;
using System.Collections.Generic;

using System;
using Xamarin.Forms;

namespace sample
{
    public partial class samplePage : ContentPage
    {

        private string[] titles = new string[]
	    {
			"地理院地図",
			"OpenStreetMap",
			"Bing Maps",
			"日本シームレス地質図"
	    };

        public samplePage()
        {
            InitializeComponent();

            Initialize();
        }

		private async void OnChangeBasemapButtonClicked(object sender, EventArgs e)
		{

			var selectedBasemap =
				await DisplayActionSheet("背景地図の選択", "キャンセル", null, titles);

			if (selectedBasemap == "Cancel")
			{
				return;
			}
			else if (selectedBasemap == "地理院地図")
			{
				var templateUri = "https://cyberjapandata.gsi.go.jp/xyz/std/{level}/{col}/{row}.png";

				var webTiledLayer = new WebTiledLayer(templateUri);

				await webTiledLayer.LoadAsync();

				if (webTiledLayer.LoadStatus == Esri.ArcGISRuntime.LoadStatus.Loaded)
				{
					MyMapView.Map.Basemap = new Basemap(webTiledLayer);

					var attribution = @"<a href=""http://maps.gsi.go.jp/development/ichiran.html"" target=""_blank"">地理院タイル</a>";
					webTiledLayer.Attribution = attribution;
				}
				else
				{
					// todo
				}
			}
			else if (selectedBasemap == "OpenStreetMap")
			{
				MyMapView.Map.Basemap = Basemap.CreateOpenStreetMap();
			}
			else if (selectedBasemap == "Bing Maps")
			{
				var bingKey = "AtuE9JPKhNEUj037BMaockdj546C6UQLdK_5cv59kCxOjJoGXteSxqysRWZseWlM";

				var bingMapLayer = new BingMapsLayer(bingKey, BingMapsLayerStyle.Road);

				MyMapView.Map.Basemap = new Basemap(bingMapLayer);

			}
			else if (selectedBasemap == "日本シームレス地質図")
			{
				var WmtsUrl = new System.Uri("https://gbank.gsj.jp/seamless/tilemap/basic/WMTSCapabilities.xml");

				WmtsService wmtsService = new WmtsService(WmtsUrl);

				await wmtsService.LoadAsync();

				if (wmtsService.LoadStatus == Esri.ArcGISRuntime.LoadStatus.Loaded)
				{
					WmtsServiceInfo wmtsServiceInfo = wmtsService.ServiceInfo;
					IReadOnlyList<WmtsLayerInfo> layerInfos = wmtsServiceInfo.LayerInfos;

					WmtsLayer wmtsLayer = new WmtsLayer(layerInfos[0]);

					MyMapView.Map.Basemap = new Basemap(wmtsLayer);
				}
				else
				{
					// todo
				}

			}

		}

		private void Initialize()
		{
			// 背景地図（地形図）の設定と初期表示の位置を設定
			Map myMap = new Map(BasemapType.OpenStreetMap, 35.6761, 139.7379, 10);

			MyMapView.Map = myMap;
		}

    }
}
