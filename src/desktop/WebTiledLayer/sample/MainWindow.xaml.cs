using System;
using System.Windows;
using System.Threading.Tasks;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Geometry;
using System.Threading;

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
            Initialize();
        }

        private void Initialize()
        {

            Map myMap = new Map();

            CustomWebTiledLayer osm = new CustomWebTiledLayer();
            myMap.Basemap.BaseLayers.Add(osm);
            MyMapView.Map = myMap;

        }

        public class CustomWebTiledLayer : Esri.ArcGISRuntime.Mapping.ServiceImageTiledLayer
        {
            public CustomWebTiledLayer()
                   : base(CreateTileInfo(), new Envelope(-20037508.3427892, -20037508.3427892, 20037508.3427892, 20037508.3427892, SpatialReferences.WebMercator))
            {
            }

            private static Esri.ArcGISRuntime.ArcGISServices.TileInfo CreateTileInfo()
            {
                var levels = new Esri.ArcGISRuntime.ArcGISServices.LevelOfDetail[19];
                double resolution = 20037508.3427892 * 2 / 256;
                double scale = resolution * 96 * 39.37;
                for (int i = 0; i < levels.Length; i++)
                {
                    levels[i] = new Esri.ArcGISRuntime.ArcGISServices.LevelOfDetail(i, resolution, scale);
                    resolution /= 2;
                    scale /= 2;
                }
                return new Esri.ArcGISRuntime.ArcGISServices.TileInfo(96, TileImageFormat.Png, levels, new MapPoint(-20037508.3427892, 20037508.3427892, SpatialReferences.WebMercator),
                     SpatialReferences.WebMercator, 256, 256);
            }


            protected override Task<Uri> GetTileUriAsync(int level, int row, int column, CancellationToken cancellationToken)
            {
                return Task.FromResult(new Uri($"http://cyberjapandata.gsi.go.jp/xyz/std/{level}/{column}/{row}.png", UriKind.Absolute));

            }
        }
    }
}
