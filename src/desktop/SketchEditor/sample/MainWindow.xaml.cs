using System;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.Geometry;
using System.Windows.Input;

namespace sample
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        SketchCreationMode creationMode;
        Geometry geometry;

        public MainWindow()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {

            Map myMap = new Map(Basemap.CreateStreets());
            MyMapView.Map = myMap;

        }

        private async void ComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if (comboBox1.SelectedIndex == 0) {
                creationMode = SketchCreationMode.Point;
            } else if (comboBox1.SelectedIndex == 1) {
                creationMode = SketchCreationMode.Polyline;
            } else if (comboBox1.SelectedIndex == 2) {
                creationMode = SketchCreationMode.Polygon;
            } else if (comboBox1.SelectedIndex == 3) {
                creationMode = SketchCreationMode.Arrow;
            } else if (comboBox1.SelectedIndex == 4) {
                creationMode = SketchCreationMode.Circle;
            } else if (comboBox1.SelectedIndex == 5) {
                creationMode = SketchCreationMode.Ellipse;
            } else if (comboBox1.SelectedIndex == 6) {
                creationMode = SketchCreationMode.Rectangle;
            } else if (comboBox1.SelectedIndex == 7) {
                creationMode = SketchCreationMode.Triangle;
            } else if (comboBox1.SelectedIndex == 8) {
                creationMode = SketchCreationMode.FreehandLine;
            } else if (comboBox1.SelectedIndex == 9) {
                creationMode = SketchCreationMode.FreehandPolygon;
            }

            try
            {
                geometry = await MyMapView.SketchEditor.StartAsync(creationMode, true);

            }
            catch (TaskCanceledException)
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {

            }

        }

        private void ComboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (comboBox2.SelectedIndex == 0)
            {
                if (MyMapView.SketchEditor.UndoCommand.CanExecute(null) == true)
                {
                    ICommand command = MyMapView.SketchEditor.UndoCommand;
                    command.Execute(geometry);
                }
            }
            else if (comboBox2.SelectedIndex == 1)
            {
                if (MyMapView.SketchEditor.RedoCommand.CanExecute(null) == true)
                {
                    ICommand command = MyMapView.SketchEditor.RedoCommand;
                    command.Execute(geometry);
                }
            }
            else if (comboBox2.SelectedIndex == 2)
            {
                if (MyMapView.SketchEditor.DeleteCommand.CanExecute(null) == true)
                {
                    ICommand command = MyMapView.SketchEditor.DeleteCommand;
                    command.Execute(geometry);
                }
            }
            else if (comboBox2.SelectedIndex == 3)
            {
                if (MyMapView.SketchEditor.CancelCommand.CanExecute(null) == true)
                {
                    ICommand command = MyMapView.SketchEditor.CancelCommand;
                    command.Execute(geometry);
                }
            }

        }


    }
}
