using System;
using System.Windows;
using System.Windows.Media;
using System.IO;

using Esri.ArcGISRuntime.LocalServices;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Tasks.Geoprocessing;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;

namespace LocalServer_GP
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {

        private LocalGeoprocessingService localGpService;
        private GraphicsOverlay resultOverlay;

        private FeatureCollectionTable pointsTable;

        public MainWindow()
        {
            InitializeComponent();

            Initialize();

            this.Dispatcher.ShutdownStarted += ShutdownSample;
            this.Unloaded += ShutdownSample;

        }

        private async void ShutdownSample(object sender, EventArgs e)
        {
            try
            {
                // アプリケーション終了時にローカル サーバーを停止
                if (LocalServer.Instance.Status == LocalServerStatus.Started)
                {
                    await LocalServer.Instance.StopAsync();
                }
            }
            catch (InvalidOperationException)
            {
                return;
            }
        }

        private async void Initialize()
        {
            // マップの背景地図と初期表示位置を設定
            MyMapView.Map = new Map(BasemapType.Topographic, 35.68151, 139.76558, 14);

            // ポイント格納用のフィーチャ コレクション テーブルの作成
            pointsTable = new FeatureCollectionTable(null, GeometryType.Point, SpatialReferences.Wgs84);

            // ポイント用のシンボルの作成
            var pointSymbol = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, Colors.Red, 8);
            // シンボルからレンダラーを作成し、フィーチャ コレクション テーブルのレンダラーに設定
            pointsTable.Renderer = new SimpleRenderer(pointSymbol);

            
            // ポイントの作成
            try
            {
                // 緯度経度のログを格納した CSV ファイルを開く
                String csvPath = Directory.GetCurrentDirectory() + @"\SampleData\PointLog.csv";
                using (var sr = new System.IO.StreamReader(csvPath))
                {
                    // CSVの行を繰り返し読み込む
                    while (!sr.EndOfStream)
                    {
                        // 読む込んだ行をカンマ毎に分けて、緯度経度を取得する
                        var line = sr.ReadLine();
                        var values = line.Split(',');
                        // 緯度経度を指定してポイントのフィーチャを作成する
                        Feature pointFeature = pointsTable.CreateFeature();
                        MapPoint point = new MapPoint(Double.Parse(values[1]), Double.Parse(values[0]), SpatialReferences.Wgs84);
                        pointFeature.Geometry = point;
                        // 作成したポイントをフィーチャ コレクション テーブルに追加
                        await pointsTable.AddFeatureAsync(pointFeature);

                    }
                }
            }

            catch (System.Exception ex)
            {
                // CSV ファイルを開くのに失敗
                MessageBox.Show(String.Format(ex.Message));
            }


            // フィーチャ コレクション テーブルからレイヤーを作成しマップに表示
            FeatureCollection featuresCollection = new FeatureCollection();
            featuresCollection.Tables.Add(pointsTable);
            FeatureCollectionLayer collectionLayer = new FeatureCollectionLayer(featuresCollection);
            MyMapView.Map.OperationalLayers.Add(collectionLayer);

            // ジオプロセシング ツールを実行した結果のラインを表示するためのシンボルを作成
            var outputLineSym = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, Colors.Blue, 4);
            var renderer = new SimpleRenderer(outputLineSym);

            // 結果のラインを表示するグラフックス オーバレイを作成し、マップに追加
            resultOverlay = new GraphicsOverlay();
            resultOverlay.Renderer = renderer;
            MyMapView.GraphicsOverlays.Add(resultOverlay);

            try
            {
                // Local Server の起動をハンドリングするための StatusChanged イベントを設定
                LocalServer.Instance.StatusChanged += ServerStatusChanged;
                // Local Server のインスタンスを起動
                await LocalServer.Instance.StartAsync();

            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(String.Format(ex.Message));
            }
        }

        private void ServerStatusChanged(object sender, StatusChangedEventArgs e)
        {
            // Local Server の起動が成功したらツール実行ボタンを有効化する
            if (e.Status == LocalServerStatus.Started)
            {
                StartBtn.IsEnabled = true;
            }
        }

        private void Button_Click_Start(object sender, RoutedEventArgs e)
        {
            // 実行結果表示用のグラフックス オーバレイに追加されているグラフィックスを消去
            resultOverlay.Graphics.Clear();
            // ローカル ジオプロセシング サービスを開始する処理
            StartLocalGpService();
            StartBtn.IsEnabled = false;
        }

        private async void StartLocalGpService()
        {
            // ジオプロセシング パッケージ ファイルのパスを取得
            String gpkPath = Directory.GetCurrentDirectory() + @"\SampleData\PointsToLine.gpk";
            //  パッケージ ファイルからローカル ジオプロセシング サービスを作成
            localGpService = new LocalGeoprocessingService(gpkPath);

            // ローカル ジオプロセシング サービスの起動をハンドリングするための StatusChanged イベントを設定
            localGpService.StatusChanged += _localGpService_StatusChanged;

            // ローカル ジオプロセシング サービスを起動
            await localGpService.StartAsync();
        }

        private async void _localGpService_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            // ジオプロセシング サービスの起動が成功したら、ジオプロセシングの処理を実行
            if (e.Status == LocalServerStatus.Started)
            {
                // ジオプロセシング サービスに含まれているツール名を指定し、実行する URL を作成する
                // "Points_To_Line" はジオプロセシング パッケージに指定したツール名
                var gpSvcUrl = localGpService.Url.AbsoluteUri + "/Points_To_Line";

                // URL を指定してジオプロセシング タスクを作成
                GeoprocessingTask _bufferTask = await GeoprocessingTask.CreateAsync(new Uri(gpSvcUrl));

                // ジオプロセシング タスクのパラメーターを設定
                // パラメーター名はジオプロセシング ツールのパラメータ名と同様
                // http://desktop.arcgis.com/ja/arcmap/latest/tools/data-management-toolbox/points-to-line.htm
                GeoprocessingParameters myParameters = new GeoprocessingParameters(GeoprocessingExecutionType.AsynchronousSubmit);
                myParameters.Inputs.Add("Input_Features", new GeoprocessingFeatures(pointsTable));

                // パラメーターを指定しジオプロセシング タスクを実行する
                // ジオプロセシング タスクのジョブを作成
                var myGpJob = _bufferTask.CreateJob(myParameters);

                try
                {
                    // ジョブ（解析）を実行し、結果を取得
                    GeoprocessingResult myAnalysisResult = await myGpJob.GetResultAsync();
                    // パラメータを指定して解析結果のフィーチャを取得
                    GeoprocessingFeatures myResultFeatures = myAnalysisResult.Outputs["Output_Feature_Class"] as GeoprocessingFeatures;
                    // フィーチャを解析結果表示用のグラフィック オーバレイに表示
                    IFeatureSet myFeatures = myResultFeatures.Features;
                    foreach (var myFeature in myFeatures)
                    {
                        resultOverlay.Graphics.Add(new Esri.ArcGISRuntime.UI.Graphic(myFeature.Geometry));
                    }
                    
                }
                catch (Exception ex)
                {
                    // ジョブが失敗したらエラー メッセージを表示
                    if (myGpJob.Status == Esri.ArcGISRuntime.Tasks.JobStatus.Failed && myGpJob.Error != null)
                        MessageBox.Show("ジオプロセシング処理の実行に失敗しました" + myGpJob.Error.Message);
                    else
                        MessageBox.Show("エラーが発生しました" + ex.ToString());
                }
                finally
                {
                    // 処理が完了したら、ローカル ジオプロセシング サービスを停止
                    await localGpService.StopAsync();

                }


            }
            else if (e.Status == LocalServerStatus.Failed)
            {
                MessageBox.Show("ジオプロセシング サービスの起動に失敗しました");

            }
            else if (e.Status == LocalServerStatus.Stopped)
            {
                MessageBox.Show("ジオプロセシング サービスが停止しました");
                StartBtn.IsEnabled = true;
            }
        }

       
    }
}
