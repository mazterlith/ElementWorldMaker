using System.Windows;
using System.Windows.Controls;
using System.Linq;

using ElementWorldMaker.Existence;

namespace ElementWorldMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public World World = new World(21);

        public MainWindow()
        {
            InitializeComponent();
            
            TabItem elementalView = Tabs.Items.OfType<TabItem>().FirstOrDefault(x => string.Compare(x.Header.ToString(), "Elemental View") == 0);
            if (elementalView != null)
                elementalView.Content = new ElementalViewer(World);

            TabItem solidView = Tabs.Items.OfType<TabItem>().FirstOrDefault(x => string.Compare(x.Header.ToString(), "Solid View") == 0);
            if (solidView != null)
                solidView.Content = new SolidViewer(World);
        }
    }
}