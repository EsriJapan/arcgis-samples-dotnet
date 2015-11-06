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
using Esri.ArcGISRuntime.Layers;

namespace ArcGISRuntime.Samples.Desktop.Pages
{
    /// <summary>
    /// AddGSILayer.xaml の相互作用ロジック
    /// </summary>
    public partial class AddGSILayer : Page
    {
        WebTiledLayer _webTiledLayer; 

        public AddGSILayer()
        {
            InitializeComponent();


            MessageBox.Show("本サンプルは、国土地理院から公開されている地理院タイルを読み込みます。\n地理院タイルの著作権は国土地理院にあり、利用に際しては、地理院の定める利用規約に従ってください。");
            
            //WebTiledLayer の取得
            _webTiledLayer = mapView.Map.Layers["GSILayer"] as WebTiledLayer;
            
            
        }

        //地理院地図の変更ボタンのイベント ハンドラ
        private void ChangeMap_Click(object sender, RoutedEventArgs e)
        {
            if (GSILayers.Visibility == System.Windows.Visibility.Visible)
                GSILayers.Visibility = System.Windows.Visibility.Collapsed;
            else
                GSILayers.Visibility = System.Windows.Visibility.Visible;
        }


        //一覧から各ボタン（地図）をクリックした際のイベント ハンドラ
        
        //標準地図
        private void std_Click(object sender, RoutedEventArgs e)
        {
            GSILayers.Visibility = System.Windows.Visibility.Collapsed;
            _webTiledLayer.TemplateUri = "http://cyberjapandata.gsi.go.jp/xyz/std/{level}/{col}/{row}.png";
        }

        //淡色地図
        private void pale_Click(object sender, RoutedEventArgs e)
        {
            GSILayers.Visibility = System.Windows.Visibility.Collapsed;
            _webTiledLayer.TemplateUri = "http://cyberjapandata.gsi.go.jp/xyz/pale/{level}/{col}/{row}.png";
        }

        //白地図
        private void blank_Click(object sender, RoutedEventArgs e)
        {
            GSILayers.Visibility = System.Windows.Visibility.Collapsed;
            _webTiledLayer.TemplateUri = "http://cyberjapandata.gsi.go.jp/xyz/blank/{level}/{col}/{row}.png";
        }

        //English
        private void english_Click(object sender, RoutedEventArgs e)
        {
            GSILayers.Visibility = System.Windows.Visibility.Collapsed;
            _webTiledLayer.TemplateUri = "http://cyberjapandata.gsi.go.jp/xyz/english/{level}/{col}/{row}.png";
        }

        //色別標高図
        private void relief_Click(object sender, RoutedEventArgs e)
        {
            GSILayers.Visibility = System.Windows.Visibility.Collapsed;
            _webTiledLayer.TemplateUri = "http://cyberjapandata.gsi.go.jp/xyz/relief/{level}/{col}/{row}.png";
        }

        //数値地図25000（土地条件）
        private void lcm25k_2012_Click(object sender, RoutedEventArgs e)
        {
            GSILayers.Visibility = System.Windows.Visibility.Collapsed;
            _webTiledLayer.TemplateUri = "http://cyberjapandata.gsi.go.jp/xyz/lcm25k_2012/{level}/{col}/{row}.png";
        }

        //電子国土基本図（オルソ画像）
        private void ort_Click(object sender, RoutedEventArgs e)
        {
            GSILayers.Visibility = System.Windows.Visibility.Collapsed;
            _webTiledLayer.TemplateUri = "http://cyberjapandata.gsi.go.jp/xyz/ort/{level}/{col}/{row}.png";
        }

        // 利用規約ボタンのイベント ハンドラ
        private void OpenTerm_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.gsi.go.jp/kikakuchousei/kikakuchousei40182.html");
        }

    }
}
