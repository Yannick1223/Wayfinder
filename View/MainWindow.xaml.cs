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
using Wayfinder.Model; //TODO: in ViewModel integrieren

namespace Wayfinder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new BaseViewModel();
            InitializeComponent();
            RenderOptions.SetBitmapScalingMode(Landscape, BitmapScalingMode.NearestNeighbor);

            ImageTests();
        }

        LandscapeRenderer rend;


        public void ImageTests()
        {
            rend = new LandscapeRenderer(100, 100);
            Landscape.Source = rend.Landscape;

            //rend.DrawFillRectangle(0, 0, 32, 32, Colors.Red);




            Random rnd = new Random();
            /*for (int x = 1; x < rend.ImageWidth; x+=rend.LandscapeTileWidth)
            {
                for (int y = 1; y < rend.ImageHeight; y+= rend.LandscapeTileHeight)
                {
                    //rend.DrawPixel(x, y, Color.FromRgb((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255)));

                    rend.DrawFillRectangle(x, y, x + rend.LandscapeTileWidth + rend.OutlineThickness, y + rend.LandscapeTileHeight + rend.OutlineThickness, Color.FromRgb((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255)));
                }
            }*/
            string path = @"C:\Users\Admin\source\repos\Wayfinder\Assets\test1.png";

            BitmapImage a = new BitmapImage();
            a.BeginInit();
            a.UriSource = new Uri(path, UriKind.Absolute);
            a.EndInit();

            WriteableBitmap test = new WriteableBitmap(a);



            Console.WriteLine("Test");
            for (int x = 1; x < rend.LandscapeRow + 1; x++)
            {
                for (int y = 1; y < rend.LandscapeColumn + 1; y++)
                {
                    rend.DrawColorTile(x, y, Color.FromRgb((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255)));
                }
            }
            rend.DrawLandscapeOutline();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            rend.DrawLandscapeOutline();
            /*Random rnd = new Random();
            for (int x = 0; x < rend.ImageWidth; x++)
            {
                for (int y = 0; y < rend.ImageHeight; y++)
                {
                    rend.DrawPixel(x, y, Color.FromRgb((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255)));
                }
            }*/
        }

        private void Landscape_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePosition = e.GetPosition(Landscape);

            // Skalierungsfaktoren berechnen
            double scaleX = Landscape.ActualWidth / Landscape.Source.Width;
            double scaleY = Landscape.ActualHeight / Landscape.Source.Height;

            // Umrechnen der Mausposition in Bildkoordinaten
            int imageX = (int)(mousePosition.X / scaleX) + 1;
            int imageY = (int)(mousePosition.Y / scaleY) + 1;

            // Ausgabe der Bildkoordinaten
            MessageBox.Show($"Die angeklickte Pixelposition im Bild ist: X={imageX}, Y={imageY}");
        }
    }
}