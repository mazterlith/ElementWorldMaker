using System.Windows;
using System.Windows.Controls;
using System.Linq;

using ElementWorldMaker.Existence;
using ElementWorldMaker.EnvironmentViewing;

namespace ElementWorldMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public World World = new World(21, 21, 21);

        public MainWindow()
        {
            InitializeComponent();

            TabItem elementalView = Tabs.Items.OfType<TabItem>().FirstOrDefault(x => string.Compare(x.Header.ToString(), "Elemental View") == 0);
            if (elementalView != null)
                elementalView.Content = new ElementalViewer(World);

            Tabs.Items.Add(new TabItem()
            {
                Header = "WaterView",
                Content = new SolidViewer(new WaterFireVM(World.Size, World.WaterFireEnvironment))
            });

            Tabs.Items.Add(new TabItem()
            {
                Header = "WindView",
                Content = new SolidViewer(new WindEarthVM(World.Size, World.WindEarthEnvironment))
            });

            Tabs.Items.Add(new TabItem()
            {
                Header = "WoodView",
                Content = new SolidViewer(new WoodMetalVM(World.Size, World.WoodMetalEnvironment))
            });

            Tabs.Items.Add(new TabItem()
            {
                Header = "EtherResistanceView",
                Content = new SolidViewer(new EtherResistanceVM(World.Size, World.EtherResistanceEnvironment))
            });

            Tabs.Items.Add(new TabItem()
            {
                Header = "EtherView",
                Content = new SolidViewer(new EtherVM(World.Size, World.EtherEnvironment))
            });
        }
    }
}