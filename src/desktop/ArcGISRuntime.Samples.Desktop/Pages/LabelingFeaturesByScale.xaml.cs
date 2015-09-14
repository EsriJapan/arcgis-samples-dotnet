using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Layers;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ArcGISRuntime.Samples.Desktop.Pages
{
    public partial class LabelingFeaturesByScale : Page
    {
        //ローカル ジオデータベース（Runtime コンテンツ）
        private const string GDB_PATH = @"..\..\..\..\..\data\runtime_contents\japan_v80_wm_pref.geodatabase";

        public LabelingFeaturesByScale()
        {
            InitializeComponent();
        }

        /// <summary>
        /// マップビュー初期化時のイベント ハンドラ
        /// </summary>
        private async void mapView_Initialized(object sender, EventArgs e)
        {
            try
            {
                //ローカル ジオデータベース（Runtime コンテンツ）を読み込みマップに追加
                var gdb = await Geodatabase.OpenAsync(GDB_PATH);
                foreach (var table in gdb.FeatureTables)
                {
                    var flayer = new FeatureLayer()
                    {
                        ID = table.Name,
                        DisplayName = table.Name,
                        FeatureTable = table
                    };

                    //フィーチャ レイヤーのラベル プロパティを初期化し赤ラベルと青ラベルを設定
                    flayer.Labeling = new LabelProperties();
                    var redLabelClass = this.layoutRoot.Resources["redLabel"] as AttributeLabelClass;
                    flayer.Labeling.LabelClasses.Add(redLabelClass);
                    var blueLabelClass = this.layoutRoot.Resources["blueLabel"] as AttributeLabelClass;
                    flayer.Labeling.LabelClasses.Add(blueLabelClass);

                    //マップにフィーチャ レイヤーを追加
                    mapView.Map.Layers.Add(flayer);
                }

                //すべてのレイヤーを初期化
                await mapView.LayersLoadedAsync();
            }
            catch(Exception ex)
            {
                MessageBox.Show(string.Format("フィーチャ レイヤーの作成に失敗しました: {0}", ex.Message));
            }
        }
    }
}
