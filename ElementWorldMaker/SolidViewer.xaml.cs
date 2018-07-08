using ElementWorldMaker.EnvironmentViewing;
using ElementWorldMaker.Existence;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ElementWorldMaker
{
    /// <summary>
    /// Interaction logic for SolidViewer.xaml
    /// </summary>
    public partial class SolidViewer : UserControl
    {
        protected IEnvironmentVM VM;

        public SolidViewer(IEnvironmentVM vm)
        {
            InitializeComponent();

            VM = vm;

            for (int i = 0; i < VM.Size.waterPosition; i++)
            {
                WorldGrid.RowDefinitions.Add(new RowDefinition()
                {
                    Height = new GridLength(1, GridUnitType.Star)
                });
            }

            for (int i = 0; i < VM.Size.woodPosition; i++)
            {
                WorldGrid.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });
            }

            for (int i = 0; i < VM.Size.windPosition; i++)
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

            for (int i = 0; i < VM.Size.waterPosition; i++)
            {
                for (int j = 0; j < VM.Size.woodPosition; j++)
                {
                    EnvironmentColor color = VM.GetColor(i, j, k);
                    Rectangle rect = new Rectangle()
                    {
                        ClipToBounds = true,
                        Stroke = new SolidColorBrush(Colors.Black),
                        Fill = new SolidColorBrush(Color.FromRgb(color.WaterColor, color.WoodColor, color.WindColor))
                    };
                    WorldGrid.Children.Add(rect);
                    Grid.SetRow(rect, i);
                    Grid.SetColumn(rect, j);
                }
            }
        }
    }
}