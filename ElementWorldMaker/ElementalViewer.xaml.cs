using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

using ElementWorldMaker.Existence;

namespace ElementWorldMaker
{
    /// <summary>
    /// Interaction logic for ElementalViewer.xaml
    /// </summary>
    public partial class ElementalViewer : UserControl
    {
        public World World;

        public ElementalViewer(World world)
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

            RigidWaterNumberDisplay.Text = World.RigidWaterWorldNumber.ToString();
            RigidWoodNumberDisplay.Text = World.RigidWoodWorldNumber.ToString();
            RigidWindNumberDisplay.Text = World.RigidWindWorldNumber.ToString();
        }

        private void RepaintWorld()
        {
            int k = PlaneDisplay.SelectedIndex;

            WorldGrid.Children.Clear();

            for (int i = 0; i < World.WaterSize; i++)
            {
                for (int j = 0; j < World.WoodSize; j++)
                {
                    int waterValue = World.WaterWorld[i, j, k];
                    int woodValue = World.WoodWorld[i, j, k];
                    int windValue = World.WindWorld[i, j, k];
                    if (waterValue < -100)
                    {
                        waterValue = -100;
                    }
                    else if (waterValue > 100)
                    {
                        waterValue = 100;
                    }
                    if (woodValue < -100)
                    {
                        woodValue = -100;
                    }
                    else if (woodValue > 100)
                    {
                        woodValue = 100;
                    }
                    if (windValue < -100)
                    {
                        windValue = -100;
                    }
                    else if (windValue > 100)
                    {
                        windValue = 100;
                    }

                    byte waterColor = (byte)((waterValue + 100) / 200.0 * byte.MaxValue);
                    byte woodColor = (byte)((woodValue + 100) / 200.0 * byte.MaxValue);
                    byte windColor = (byte)((windValue + 100) / 200.0 * byte.MaxValue);

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

        private void PlaneDisplay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RepaintWorld();
        }

        private void RigidWaterNumberDisplay_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            TryUpdateRigidWaterNumber();

            Keyboard.ClearFocus();
        }

        private void RigidWaterNumberDisplay_LostFocus(object sender, RoutedEventArgs e)
        {
            TryUpdateRigidWaterNumber();
        }

        private void TryUpdateRigidWaterNumber()
        {
            if (string.IsNullOrWhiteSpace(RigidWaterNumberDisplay.Text))
                return;

            double rigidWaterNumber;
            if (!double.TryParse(RigidWaterNumberDisplay.Text, out rigidWaterNumber))
            {
                MessageBox.Show("Rigid Water Number needs to be a double precision value.");
            }

            if (rigidWaterNumber <= 0)
            {
                rigidWaterNumber = 0;
                RigidWaterNumberDisplay.Text = World.RigidWaterWorldNumber.ToString();
            }

            World.RigidWaterWorldNumber = rigidWaterNumber;
        }

        private void RigidWoodNumberDisplay_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            TryUpdateRigidWoodNumber();

            Keyboard.ClearFocus();
        }

        private void RigidWoodNumberDisplay_LostFocus(object sender, RoutedEventArgs e)
        {
            TryUpdateRigidWoodNumber();
        }

        private void TryUpdateRigidWoodNumber()
        {
            if (string.IsNullOrWhiteSpace(RigidWoodNumberDisplay.Text))
                return;

            double rigidWoodNumber;
            if (!double.TryParse(RigidWoodNumberDisplay.Text, out rigidWoodNumber))
            {
                MessageBox.Show("Rigid Water Number needs to be a double precision value.");
            }

            if (rigidWoodNumber <= 0)
            {
                rigidWoodNumber = 0;
                RigidWoodNumberDisplay.Text = World.RigidWoodWorldNumber.ToString();
            }

            World.RigidWoodWorldNumber = rigidWoodNumber;
        }

        private void RigidWindNumberDisplay_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            TryUpdateRigidWindNumber();

            Keyboard.ClearFocus();
        }

        private void RigidWindNumberDisplay_LostFocus(object sender, RoutedEventArgs e)
        {
            TryUpdateRigidWindNumber();
        }

        private void TryUpdateRigidWindNumber()
        {
            if (string.IsNullOrWhiteSpace(RigidWindNumberDisplay.Text))
                return;

            double rigidWindNumber;
            if (!double.TryParse(RigidWindNumberDisplay.Text, out rigidWindNumber))
            {
                MessageBox.Show("Rigid Water Number needs to be a double precision value.");
            }

            if (rigidWindNumber <= 0)
            {
                rigidWindNumber = 0;
                RigidWindNumberDisplay.Text = World.RigidWindWorldNumber.ToString();
            }

            World.RigidWindWorldNumber = rigidWindNumber;
        }

        private void UpdateWorld_Click(object sender, RoutedEventArgs e)
        {
            World.RemakeElementalMap();
            RepaintWorld();
        }
    }
}