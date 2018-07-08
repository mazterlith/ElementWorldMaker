using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

using ElementWorldMaker.Existence;

namespace ElementWorldMaker
{
    /// <summary>
    /// Interaction logic for SolidViewer.xaml
    /// </summary>
    public partial class SolidViewer : UserControl
    {
        public World World;

        public SolidViewer(World world)
        {
            InitializeComponent();

            World = world;

            for (int i = 0; i < World.WaterSize; i++)
            {
                WorldGrid.RowDefinitions.Add(new RowDefinition()
                {
                    Height = new GridLength(1, GridUnitType.Star)
                });
            }

            for (int i = 0; i < World.WoodSize; i++)
            {
                WorldGrid.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });
            }

            for (int i = 0; i < World.WindSize; i++)
            {
                PlaneDisplay.Items.Add(i);
            }

            PlaneDisplay.SelectedIndex = 0;
        }

        private void PlaneDisplay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RepaintWorld();
        }

        private void RepaintWorld()
        {
            int k = PlaneDisplay.SelectedIndex;

            WorldGrid.Children.Clear();

            for (int i = 0; i < World.WaterSize; i++)
            {
                for (int j = 0; j < World.WoodSize; j++)
                {
                    double airLevel = ((int)World.WindEarthEnvironment[i, j, k] + 2) / 4.0;

                    byte waterColor = (byte)(airLevel * byte.MaxValue);
                    byte woodColor = (byte)(airLevel * byte.MaxValue);
                    byte windColor = (byte)(airLevel * byte.MaxValue);

                    Rectangle rect = new Rectangle();
                    rect.ClipToBounds = true;
                    rect.Stroke = new SolidColorBrush(Colors.Black);
                    rect.Fill = new SolidColorBrush(Color.FromRgb(windColor, woodColor, waterColor));
                    WorldGrid.Children.Add(rect);
                    Grid.SetRow(rect, i);
                    Grid.SetColumn(rect, j);
                }
            }
        }
    }
}