using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Tasks.Geocoding;
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


namespace sample
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //ArcGIS Online ジオコーディングサービスの URL
        private const string WORLD_GEOCODE_SERVICE_URL = "http://geocode.arcgis.com/arcgis/rest/services/World/GeocodeServer";
        
        //住所検索結果表示用のグラフィックスオーバーレイ
        private GraphicsOverlay geocodeResultGraphicsOverlay;

        //住所検索用のジオコーディング タスク  
        private LocatorTask onlineLocatorTask;


        private async void MyMapView_Loaded(object sender, RoutedEventArgs e)
        {
            //住所検索用のジオコーディング タスクを初期化
            onlineLocatorTask = await LocatorTask.CreateAsync(new Uri(WORLD_GEOCODE_SERVICE_URL));

            // Create new Map with basemap
            Map myMap = new Map(Basemap.CreateImagery());

            // Assign the map to the MapView
            MyMapView.Map = myMap;

            //MapView コントロールの Map プロパティに、Map を割り当て
            MyMapView.Map = myMap;

            // グラフィックス オーバーレイが存在しない場合は、新規に追加
            if (MyMapView.GraphicsOverlays.Count == 0)
            {
                geocodeResultGraphicsOverlay = new GraphicsOverlay()
                {
                    Renderer = createGeocoordingSymbol(),
                };
                MyMapView.GraphicsOverlays.Add(geocodeResultGraphicsOverlay);
            }

        }

        private async void geocoording_Click(object sender, RoutedEventArgs e)
        {

            //住所検索用のパラメータを作成
            var geocodeParams = new GeocodeParameters
            {
                MaxResults = 5,
                OutputSpatialReference = SpatialReferences.WebMercator,
                CountryCode = "Japan",
                OutputLanguage = new System.Globalization.CultureInfo("ja-JP")
            };

            try
            {
                //住所の検索
                var resultCandidates = await onlineLocatorTask.GeocodeAsync(addressTextBox.Text, geocodeParams);

                //住所検索結果に対する処理（1つ以上候補が返されていれば処理を実行）
                if (resultCandidates != null && resultCandidates.Count > 0)
                {
                    //現在の結果を消去
                    geocodeResultGraphicsOverlay.Graphics.Clear();

                    //常に最初の候補を採用
                    var candidate = resultCandidates.FirstOrDefault();
                    
                    //最初の候補からグラフィックを作成
                    Graphic locatedPoint = new Graphic()
                    {
                        Geometry = candidate.DisplayLocation,
                    };

                    //住所検索結果表示用のグラフィックスオーバーレイにグラフィックを追加
                    geocodeResultGraphicsOverlay.Graphics.Add(locatedPoint);

                    //追加したグラフィックの周辺に地図を拡大
                    await MyMapView.SetViewpointCenterAsync((MapPoint)locatedPoint.Geometry, 36112);
                }
                //候補が一つも見つからない場合の処理
                else
                {
                    MessageBox.Show("住所検索：該当する場所がみつかりません。");
                }
            }
            //エラーが発生した場合の処理
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("住所検索：{0}", ex.Message));
            }
        }

        private SimpleRenderer createGeocoordingSymbol()
        {
            SimpleMarkerSymbol resultGeocoordingSymbol = new SimpleMarkerSymbol()
            {
                Style = SimpleMarkerSymbolStyle.Circle,
                Size = 12,
                Color = Colors.Blue,
            };

            SimpleRenderer resultRenderer = new SimpleRenderer() { Symbol = resultGeocoordingSymbol };

            return resultRenderer;
        }

    }
}
