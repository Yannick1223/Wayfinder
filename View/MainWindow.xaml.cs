using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wayfinder.ViewModel;
using Wayfinder.Model;

namespace Wayfinder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LandscapeViewModel LandscapeVM;

        public MainWindow()
        {
            LandscapeVM = new LandscapeViewModel();
            DataContext = LandscapeVM;
            InitializeComponent();

            Init();
        }

        private void Init()
        {
            SetTileSelection();
            SetLandscapeImage();
            SetBitmapScaling();
        }

        private void SetTileSelection()
        {
            LandscapeVM.SetTileSelection(TileSelection);
        }

        private void SetLandscapeImage()
        {
            LandscapeVM.SetLandscapeImage(Landscape);
        }

        private void SetBitmapScaling()
        {
            RenderOptions.SetBitmapScalingMode(Landscape, BitmapScalingMode.NearestNeighbor);
            RenderOptions.SetBitmapScalingMode(TileSelection, BitmapScalingMode.NearestNeighbor);
        }
    }
}