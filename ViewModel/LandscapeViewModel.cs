using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wayfinder.Model;

namespace Wayfinder.ViewModel
{
    public partial class LandscapeViewModel: BaseViewModel
    {
        public LandscapeViewModel()
        {
            Title = "Wayfinder";
            
            ObservableTileInformation = new ObservableCollection<TileInformation>();
            Handler = new LandscapeHandler(100, 100, 8, 8, 1); //TODO: Change width, height to 16

            InitObservableTileInformation();
        }

        public void SetTileSelection(ListBox _selection)
        {
            SelectedItem = _selection;
        }

        public void InitObservableTileInformation()
        {
            ObservableCollection<TileInformation> tiles = Handler.GetObservableTiles();

            foreach(TileInformation tile in tiles)
            {
                ObservableTileInformation.Add(tile);
            }
        }

        public LandscapeHandler Handler { get; private set; }
        public Image Landscape { get; private set; }

        public ObservableCollection<TileInformation> ObservableTileInformation { get; set; }

        public ListBox SelectedItem;

        public void SetLandscapeImage(Image _landscape)
        {
            Landscape = _landscape;
            SetLandscapeToImageControl();
        }

        public void SetLandscapeToImageControl()
        {
            Landscape.Source = Handler.Renderer.Landscape;
        }


        [RelayCommand]
        public void OnMouseMoveOverImage(MouseEventArgs e)
        {
            
            if (e.LeftButton.Equals(MouseButtonState.Pressed))
            {
                OnClickedLandscape(e);
            }
        }


        public void OnClickedLandscape(MouseEventArgs e)
        {
            Point mousePosition = e.GetPosition(Landscape);

            double scaleX = Landscape.ActualWidth / Landscape.Source.Width;
            double scaleY = Landscape.ActualHeight / Landscape.Source.Height;

            int imageX = (int)(mousePosition.X / scaleX) + 1;
            int imageY = (int)(mousePosition.Y / scaleY) + 1;

            //MessageBox.Show($"Die angeklickte Pixelposition im Bild ist: X={imageX}, Y={imageY}");

            Point? tilePos = Handler.GetTileFromPosition(imageX, imageY);

            if (tilePos != null)
            {
                TileType? currentSelectedTile = GetCurentSelectedTile();

                if(currentSelectedTile != null)
                {
                    Handler.DrawTileAtPosition((int)tilePos.Value.X, (int)tilePos.Value.Y, currentSelectedTile.Value);
                }

                //MessageBox.Show($"Die angeklickte TilePosition im Bild ist: row={tilePos.Value.X}, col={tilePos.Value.Y}");
            }
        }

        private TileType? GetCurentSelectedTile()
        {
            TileType? result = null;

            result = (SelectedItem.SelectedItem as TileInformation)?.Type;

            return result;
        }
    }
}
