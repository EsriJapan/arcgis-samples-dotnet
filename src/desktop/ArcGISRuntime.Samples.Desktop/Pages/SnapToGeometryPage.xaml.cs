//本サンプルは以下の GeoNet のスレッドのサンプルを一部変更したものです。
//https://geonet.esri.com/message/452748#452748

using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Symbology;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ArcGISRuntime.Samples.Desktop.Pages
{
    public partial class SnapToGeometryPage : Page
    {
        private double snapToleranceByPixel = 20;           //スナップ許容値（ピクセル数）
        private GraphicsOverlay toleranceOverlay;           //スナップ許容範囲表示用のマップオーバーレイ
        private GraphicsOverlay snapOverlay;                //スナップ エフェクト表示用のマップオーバーレイ
        private string snapMode = "geometry";               //スナップモード

        public SnapToGeometryPage()
        {
            InitializeComponent();

            //サンプル ポリゴンの追加
            var layer = (GraphicsLayer)mapView.Map.Layers["sampleDataLayer"];
            Graphic g = new Graphic(new Polygon(new MapPoint[] { new MapPoint(-20, -20),
                                                                 new MapPoint(20, -20),
                                                                 new MapPoint(20, 20),
                                                                 new MapPoint(0, 25),
                                                                 new MapPoint(-20, 20) },
                                                SpatialReferences.Wgs84),
                                                new SimpleLineSymbol() { Width = 2, Color = Colors.Blue });
            layer.Graphics.Add(g);

            //スナップ エフェクト表示用のマップオーバーレイを取得
            this.snapOverlay = mapView.GraphicsOverlays["snapOverlay"];

            //スナップ エフェクト表示用のグラフィックを追加
            this.snapOverlay.Graphics.Add(new Graphic()
            {
                Symbol = new SimpleMarkerSymbol()
                {
                    Color = Colors.Transparent,
                    Size = 10,
                }
            });

            //スナップ許容範囲表示用のマップオーバーレイを取得
            this.toleranceOverlay = mapView.GraphicsOverlays["toleranceOverlay"];

            //スナップ許容範囲表示用のグラフィックを追加
            this.toleranceOverlay.Graphics.Add(new Graphic()
            {
                Symbol = new SimpleMarkerSymbol()
                {
                    Color = Color.FromArgb(75, 150, 150, 255),
                    Size = snapToleranceByPixel * 2,
                }
            });
        }

        /// <summary>
        /// マウス移動時のイベント ハンドラ
        /// </summary>
        private void mapView_MouseMove(object sender, MouseEventArgs e)
        {
            //マウスの位置を座標値に変換
            var mouseLocation = mapView.ScreenToLocation(e.GetPosition(mapView));

            //マウスの位置（座標値）に許容範囲表示用グラフィックを移動
            this.toleranceOverlay.Graphics[0].Geometry = mouseLocation;

            //現在のマウス位置（座標値）でスナップ処理を実行
            Snap(mouseLocation, mapView.UnitsPerPixel);
        }

        /// <summary>
        /// 現在のマウス位置（座標値）でスナップ処理を実行
        /// </summary>
        private void Snap(Esri.ArcGISRuntime.Geometry.MapPoint mapPoint, double mapResolution)
        {
            if (mapPoint == null) return;

            //サンプル ポリゴンのジオメトリを取得
            var layer = (GraphicsLayer)mapView.Map.Layers["sampleDataLayer"];
            var geometry = layer.Graphics[0].Geometry;

            //スナップ許容値（距離）= 現在のマップの 1 ピクセルあたりの距離 * スナップ許容値（ピクセル数）
            var tolerance = mapResolution * this.snapToleranceByPixel;

            //スナップ対象のジオメトリの空間参照をマップの空間参照に一致
            if (mapPoint.SpatialReference.Wkid != geometry.SpatialReference.Wkid)
            {
                geometry = GeometryEngine.Project(geometry, mapPoint.SpatialReference);
            }
            
            //スナップ処理を実行
            SnapResult snapResult = null;
            switch(this.snapMode)
            {
                case "geometry":
                    snapResult = ExecuteSnap(mapPoint, geometry, SnapType.Vertex | SnapType.Segment, tolerance);
                    break;
                case "vertex":
                    snapResult = ExecuteSnap(mapPoint, geometry, SnapType.Vertex, tolerance);
                    break;
                case "segment":
                    snapResult = ExecuteSnap(mapPoint, geometry, SnapType.Segment, tolerance);
                    break;
            }

            //スナップ結果が存在しない場合はスナップ エフェクトを非表示
            if(snapResult == null)
            {
                this.snapOverlay.IsVisible = false;
                this.snapOverlay.Graphics[0].Geometry = null;
                return;
            }

            //スナップ エフェクトを表示
            this.snapOverlay.IsVisible = true;
            this.snapOverlay.Graphics[0].Geometry = snapResult.Result.Point;

            //スナップ エフェクトのシンボル色を頂点（赤）と線分（白）で変更
            if (snapResult.Type == SnapType.Vertex)
            {
                (this.snapOverlay.Graphics[0].Symbol as SimpleMarkerSymbol).Color = Colors.Red;
            }
            else
            {
                (this.snapOverlay.Graphics[0].Symbol as SimpleMarkerSymbol).Color = Colors.White;
            }
        }

        /// <summary>
        /// スナップ処理を実行
        /// </summary>
        private static SnapResult ExecuteSnap(MapPoint input, Esri.ArcGISRuntime.Geometry.Geometry snapToGeometry, SnapType type, double vertexSnapTolerance)
        {
            //スナップ対象として頂点を使用する場合
            if ((type & SnapType.Vertex) == SnapType.Vertex)
            {
                //もっとも近いスナップ対象のジオメトリの頂点（スナップ対象の頂点）を取得
                var vertexResult = GeometryEngine.NearestVertex(snapToGeometry, input);

                //スナップ対象の頂点までの距離がスナップ許容値内の場合は値を返す
                if (vertexResult.Distance <= vertexSnapTolerance)
                {
                    return new SnapResult()
                    {
                        Result = vertexResult,
                        Type = SnapType.Vertex
                    };
                }
            }

            //スナップ対象として線分を使用する場合
            if ((type & SnapType.Segment) == SnapType.Segment)
            {
                //内部交差を避けるためポリゴンの外周をポリラインに変換
                var outline = snapToGeometry;
                if (outline is Polygon)
                {
                    outline = new Polyline(((Polygon)outline).Parts, outline.SpatialReference);
                }

                //もっとも近いスナップ対象の線分上の位置を取得
                var segmentResult = GeometryEngine.NearestCoordinate(outline, input);

                //スナップ対象の線分上の位置までの距離がスナップ許容値内の場合は値を返す
                if (segmentResult.Distance <= vertexSnapTolerance)
                {
                    return new SnapResult()
                    {
                        Result = segmentResult,
                        Type = SnapType.Segment
                    };
                }
                    
            }

            //検索結果までの距離がスナップ許容値外の場合は結果を返さない
            return null;
        }

        /// <summary>
        /// スナップ モードボタンクリック時のイベント ハンドラ
        /// </summary>
        private void SnapModeRadioButton_Click(object sender, RoutedEventArgs e)
        {
            //スナップ モードを変更
            this.snapMode = ((RadioButton)sender).Tag.ToString();
        }
    }

    /// <summary>
    /// スナップの実行結果
    /// </summary>
    public class SnapResult
    {
        public SnapType Type { get; internal set; }
        public ProximityResult Result { get; set; }
    }

    /// <summary>
    /// スナップ タイプ
    /// </summary>
    [Flags]
    public enum SnapType
    {
        None = 0,
        Vertex = 1,
        Segment = 2
    }
}
