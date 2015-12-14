using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Layers;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ArcGISRuntime.Samples.Desktop.Pages
{
    public partial class FilterFeatureLayer : Page
    {
        //ローカル ジオデータベース（Runtime コンテンツ）
        private const string GDB_PATH = @"..\..\..\..\..\data\runtime_contents\japan_v80_wm_pref.geodatabase";

        private GraphicsLayer _graphicsLayer;           //フィーチャ表示用グラフィックスレイヤー
        private GeodatabaseFeatureTable _gdbTable;      //フィーチャ テーブル

        public FilterFeatureLayer()
        {
            InitializeComponent();
        }

        /// <summary>
        /// マップビュー読み込み時のイベント ハンドラ
        /// </summary>
        private async void mapView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //ローカル ジオデータベース（Runtime コンテンツ）のフィーチャ テーブルを読み込み
                var gdb = await Geodatabase.OpenAsync(GDB_PATH);
                _gdbTable = gdb.FeatureTables.FirstOrDefault();

                //フィーチャ テーブルに設定されたレンダラ―（シンボル情報）を取得
                _graphicsLayer = new GraphicsLayer() { RenderingMode = GraphicsRenderingMode.Static };
                _graphicsLayer.Renderer = _gdbTable.ServiceInfo.DrawingInfo.Renderer;

                //フィーチャを表示
                var features = await _gdbTable.QueryAsync(new QueryFilter() { WhereClause = GetFilterTest() });
                _graphicsLayer.GraphicsSource = features.Select(f => new Graphic(f.Geometry));

                //マップにグラフィックスレイヤーを追加
                mapView.Map.Layers.Add(_graphicsLayer);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("フィーチャ レイヤーの作成に失敗しました: {0}", ex.Message));
            }
        }

        /// <summary>
        /// フィルターボタンクリック時
        /// </summary>
        private async void filterButton_Click(object sender, RoutedEventArgs e)
        {
            //既存のグラフィックスを削除
            _graphicsLayer.GraphicsSource = null;

            //フィーチャを表示
            var features = await _gdbTable.QueryAsync(new QueryFilter() { WhereClause = GetFilterTest() });
            _graphicsLayer.GraphicsSource = features.Select(f => new Graphic(f.Geometry));
        }

        /// <summary>
        /// フィルター文字列を生成
        /// </summary>
        private string GetFilterTest()
        {
            if (string.IsNullOrEmpty(pNumTextBox.Text))
            {
                return "1=1";
            }

            int pNum;
            if (int.TryParse(pNumTextBox.Text, out pNum))
            {
                return "SUM_P_NUM >= " + pNumTextBox.Text;
            }
            else
            {
                pNumTextBox.Text = string.Empty;
                MessageBox.Show("不正な入力値です");
                return "1=1";
            }
        }
    }
}
