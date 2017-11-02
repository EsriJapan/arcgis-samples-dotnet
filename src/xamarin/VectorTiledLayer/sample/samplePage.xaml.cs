using Xamarin.Forms;
using System;

using Esri.ArcGISRuntime.Mapping;

namespace sample
{
    public partial class samplePage : ContentPage
    {

        // ArcGIS Online のアイテム URL
        private string _mapstyler1Url = "https://www.arcgis.com/home/item.html?id=53198d7dc1ac458086a7da43f409260f";
        private string _mapstyler2Url = "https://www.arcgis.com/home/item.html?id=4f8e8d131bbd488b822d258a8218cfae";
        private string _mapstyler3Url = "https://www.arcgis.com/home/item.html?id=1d7cb019befc43ed86f6c7631d9315dd";
        private string _mapstyler4Url = "https://www.arcgis.com/home/item.html?id=02224acbcbfc4e238fdb11b4f1a6924e";

        private string _vectorTiledLayerUrl;
        private ArcGISVectorTiledLayer _vectorTiledLayer;

        // ベクター タイルの選択肢を格納する配列
        private string[] _vectorLayerNames = new string[]
        {
            "VectorTiledLayer 1",
            "VectorTiledLayer 2",
            "VectorTiledLayer 3",
            "VectorTiledLayer 4"
        };

        public samplePage()
        {
            InitializeComponent();

            Initialize();
        }

        private void Initialize()
        {
  
            // URL を指定して新しい ArcGISVectorTiledLayer のインスタンスを作成
            _vectorTiledLayer = new ArcGISVectorTiledLayer(new Uri(_mapstyler1Url));

            // ベクター タイルを使用して新しいベースマップを作成
            Map myMap = new Map(new Basemap(_vectorTiledLayer));

            // 作成したマップを MapView に割り当て
            MyMapView.Map = myMap;

        }

        private async void OnChangeLayerButtonClicked(object sender, EventArgs e)
        {
            // ベクター タイルの選択画面を表示し、選択画面から任意のベクター タイルを選択
            var selectedLayer =
                await DisplayActionSheet("Select basemap", "Cancel", null, _vectorLayerNames);

            // キャンセルを選択した場合は何もしない
            if (selectedLayer == "Cancel") return;

            switch (selectedLayer)
            {
                case "VectorTiledLayer 1":
                    _vectorTiledLayerUrl = _mapstyler1Url;
                    break;

                case "VectorTiledLayer 2":
                    _vectorTiledLayerUrl = _mapstyler2Url;
                    break;

                case "VectorTiledLayer 3":
                    _vectorTiledLayerUrl = _mapstyler3Url;
                    break;

                case "VectorTiledLayer 4":
                    _vectorTiledLayerUrl = _mapstyler4Url;
                    break;

                default:
                    break;
            }

            // ユーザーが選択した URL で新しい ArcGISVectorTiledLayer のインスタンスを作成
            _vectorTiledLayer = new ArcGISVectorTiledLayer(new Uri(_vectorTiledLayerUrl));

            // 新しいベクター タイルを使用してベースマップを作成し、Mapview に割り当て
            MyMapView.Map.Basemap = new Basemap(_vectorTiledLayer);

        }

    }
}
