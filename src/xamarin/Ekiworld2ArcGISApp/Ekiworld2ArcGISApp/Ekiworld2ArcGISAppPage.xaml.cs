using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using Xamarin.Forms;

using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.UI.Controls;

#if WINDOWS_UWP
using Colors = Windows.UI.Colors;
#else
using Colors = System.Drawing.Color;
#endif

namespace Ekiworld2ArcGISApp
{
    public partial class Ekiworld2ArcGISAppPage : ContentPage
    {

        public Ekiworld2ArcGISAppPage()
        {
            InitializeComponent();

            this.Title = "Ekiworld2ArcGIS";

            getEkiworldBtn.Clicked += GetEkiworldBtn_Clicked;

            Initialize();

        }

        private void Initialize()
        {
            Map myMap = new Map(
                    BasemapType.Streets,
                    35.677349,
                    139.737464,
                    10
                );

            MyMapView.Map = myMap;
        }

        private async void GetEkiworldBtn_Clicked(object sender, EventArgs args)
        {
            if (!string.IsNullOrEmpty(EkiEntry.Text))
            {

                var Ekis = await Core.GetEkiworldResult(EkiEntry.Text);

                if (Ekis == null) 
                {
                    DisplayAlert("通知","検索に失敗しました。検索項目を変更して再度検索してください","OK");
                    return;
                }
                else 
                {
                    // フィーチャ コレクションのインスタンス化  ※ フィーチャ コレクションとは、ジオメトリや属性情報の地物として作成したフィーチャのリストになります。
                    FeatureCollection featuresCollection = new FeatureCollection();

                    foreach (Ekiworld Eki in Ekis) 
                    {

                        // フィールド情報を定義
                        List<Field> pointFields = new List<Field>();

                        Field stationName = new Field(FieldType.Text, "stationName", Eki.stationName, 50);
                        Field stationCode = new Field(FieldType.Text, "stationCode", Eki.stationCode, 50);
                        Field stationYomi = new Field(FieldType.Text, "stationYomi", Eki.stationYomi, 50);
                        Field stationType = new Field(FieldType.Text, "stationType", Eki.stationType, 50);
                        Field prefName = new Field(FieldType.Text, "prefName", Eki.prefName, 50);
                        Field longi_d = new Field(FieldType.Float64, "longi_d", Eki.longi_d.ToString(), 50);
                        Field lati_d = new Field(FieldType.Float64, "lati_d", Eki.lati_d.ToString(), 50);

                        pointFields.Add(stationName);
                        pointFields.Add(stationCode);
                        pointFields.Add(stationYomi);
                        pointFields.Add(stationType);
                        pointFields.Add(prefName);
                        pointFields.Add(longi_d);
                        pointFields.Add(lati_d);
                        // 作成したフィールド情報やジオメトリタイプからフィーチャ コレクションテーブルをインスタンス化
                        FeatureCollectionTable pointsTable = new FeatureCollectionTable(pointFields, GeometryType.Point, SpatialReferences.Wgs84);

                        pointsTable.Renderer = CreateRenderer(GeometryType.Point);

                        // 新しいポイント フィーチャを作成し、ジオメトリと属性値を設定
                        Feature pointFeature = pointsTable.CreateFeature();
                        pointFeature.SetAttributeValue(stationName, Eki.stationName);
                        pointFeature.SetAttributeValue(stationYomi, Eki.stationYomi);
                        pointFeature.SetAttributeValue(prefName, Eki.prefName);

                        MapPoint point = new MapPoint(Eki.longi_d, Eki.lati_d, SpatialReferences.Wgs84);
                        pointFeature.Geometry = point;

                        await pointsTable.AddFeatureAsync(pointFeature);
                        // フィーチャ コレクションにフィーチャ コレクションテーブルを追加
                        featuresCollection.Tables.Add(pointsTable);

                    }
                    // フィーチャ コレクションからフィーチャ コレクションレイヤーをインスタンス化
                    FeatureCollectionLayer collectionLayer = new FeatureCollectionLayer(featuresCollection);

                    collectionLayer.Loaded += (s, e) => Device.BeginInvokeOnMainThread(async () =>
                    {
                        await MyMapView.SetViewpointAsync(new Viewpoint(collectionLayer.FullExtent));
                    });

                    if (MyMapView.Map.OperationalLayers.Count > 0)
                    {
                        MyMapView.Map.OperationalLayers.RemoveAt(0);
                    }
                    // フィーチャ コレクションレイヤーを Map に追加
                    MyMapView.Map.OperationalLayers.Add(collectionLayer);

                    MyMapView.GeoViewTapped += OnMapViewTapped;

                }

            }

        }

        private async void OnMapViewTapped(object sender, Esri.ArcGISRuntime.Xamarin.Forms.GeoViewInputEventArgs e) {
            
            var layer = MyMapView.Map.OperationalLayers[0]; // レイヤーの指定
            var pixelTolerance = 10; // クリック地点からの許容範囲
            var returnPopupsOnly = false; //ポップアップの要素だけを含めるか
            var maxResults = 1; // レスポンス結果の MAX値
            // IdentifyLayerAsync メソッドの定義
            var idResults = await MyMapView.IdentifyLayerAsync(layer, e.Position, pixelTolerance, returnPopupsOnly, maxResults);

            var stationName = "";
            var stationYomi = "";
            var prefName = "";
            var lati_d = "";
            var longi_d = "";

            if (idResults.SublayerResults.Count > 0) {

                foreach (var sr in idResults.SublayerResults) {
                    // SublayerResults 内の GeoElements からフィーチャの情報を取得
                    foreach (GeoElement idElement in sr.GeoElements)
                    {
                        Feature idFeature = idElement as Feature;

                        stationName = idFeature.FeatureTable.GetField("stationName").Alias;
                        stationYomi = idFeature.FeatureTable.GetField("stationYomi").Alias;
                        prefName = idFeature.FeatureTable.GetField("prefName").Alias;
                        lati_d = idFeature.FeatureTable.GetField("lati_d").Alias;
                        longi_d = idFeature.FeatureTable.GetField("longi_d").Alias;

                    }

                }

                string mapLocationDescription = string.Format("Lat:{0:F3} Long:{1:F3}", lati_d, longi_d);
                // 駅情報の座標を指定して、CalloutDefinition をインスタンス化
                CalloutDefinition myCalloutDefinition = new CalloutDefinition("Location:", mapLocationDescription);
                // myCalloutDefinition に ポップアップに表示させたい内容を指定（Text、DetailText 、BttonImage）
                myCalloutDefinition.Text = stationName + " : " + prefName;
                myCalloutDefinition.DetailText = stationYomi;

                RuntimeImage rtImg = new RuntimeImage(new Uri("https://cdn0.iconfinder.com/data/icons/business-finance-vol-14-2/512/69-128.png"));
                try
                {
                    await rtImg.LoadAsync();
                } catch (Exception ex) {
                    Console.WriteLine(ex);
                }
                myCalloutDefinition.ButtonImage = rtImg;
                // クリック地点の座標と、ポップアップに表示する内容を指定することで、ShowCalloutAt が呼ばれる
                MyMapView.ShowCalloutAt(e.Location, myCalloutDefinition);

            }
           
        }

        private Renderer CreateRenderer(GeometryType rendererType)
        {
            Symbol sym = null;

            switch (rendererType)
            {
                case GeometryType.Point:
                case GeometryType.Multipoint:
                    // Create a marker symbol
                    sym = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Triangle, Colors.Red, 18);
                    break;
                case GeometryType.Polyline:
                    // Create a line symbol
                    sym = new SimpleLineSymbol(SimpleLineSymbolStyle.Dash, Colors.Green, 3);
                    break;
                case GeometryType.Polygon:
                    // Create a fill symbol
                    var lineSym = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, Colors.DarkBlue, 2);
                    sym = new SimpleFillSymbol(SimpleFillSymbolStyle.DiagonalCross, Colors.Cyan, lineSym);
                    break;
                default:
                    break;
            }

            return new SimpleRenderer(sym);
        }

    }
}
