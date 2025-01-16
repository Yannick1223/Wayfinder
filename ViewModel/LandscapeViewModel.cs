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
using System.Windows.Navigation;
using Wayfinder.Model;

namespace Wayfinder.ViewModel
{
    public partial class LandscapeViewModel: BaseViewModel
    {
        public LandscapeHandler Handler { get; private set; }
        public Image Landscape { get; private set; }

        public ObservableCollection<TileInformation> ObservableTileInformation { get; private set; }

        private ListBox SelectedItem { get; set; }

        public LandscapeViewModel()
        {
            Title = "Wayfinder";

            Handler = new LandscapeHandler(100, 100, 8, 8, 1);
            ObservableTileInformation = new ObservableCollection<TileInformation>();

            UpdateObservableSelection();
        }

        public void UpdateObservableSelection()
        {
            foreach (TileInformation tile in Handler.GetObservableTiles())
            {
                ObservableTileInformation.Add(tile);
            }
        }

        public void SetTileSelection(ListBox _selection)
        {
            SelectedItem = _selection;
        }

        public void SetLandscapeImage(Image _landscape)
        {
            Landscape = _landscape;
            SetLandscapeToImageControl();
        }

        private void SetLandscapeToImageControl()
        {
            Landscape.Source = Handler.Renderer.Landscape;
        }


        [RelayCommand]
        public void OnHoverOverImage(MouseEventArgs e)
        {
            DrawTile(e);
        }

        private void DrawTile(MouseEventArgs e)
        {
            if (PressedLeftButton(e))
            {
                DrawTileAtMousePosition(e, GetCurentSelectedTile());
            }
            else if (PressedRightButton(e))
            {
                DrawTileAtMousePosition(e, TileType.Water);
            }
        }

        private void DrawTileAtMousePosition(MouseEventArgs e, TileType? _type)
        {
            Point imagePos = MousePositionToImagePosition(e);
            Point? tilePos = ImagePositionToTilePosition(imagePos);

            if(tilePos != null && _type != null)
            {
                Handler.DrawTileAtPosition((int)tilePos.Value.X, (int)tilePos.Value.Y, _type.Value);
            }
        }

        private Point MousePositionToImagePosition(MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(Landscape);
            Point imageScale = new Point(Landscape.ActualWidth / Landscape.Source.Width, Landscape.ActualHeight / Landscape.Source.Height);

            return new Point((int)(mousePos.X / imageScale.X) + 1, (int)(mousePos.Y / imageScale.Y) + 1);
        }

        private Point? ImagePositionToTilePosition(Point _imagePos)
        {
            return Handler.GetTileFromPosition(_imagePos);
        }

        private TileType? GetCurentSelectedTile()
        {
            return (SelectedItem.SelectedItem as TileInformation)?.Type;
        }

        private bool PressedLeftButton(MouseEventArgs e)
        {
            return e.LeftButton.Equals(MouseButtonState.Pressed);
        }

        private bool PressedRightButton(MouseEventArgs e)
        {
            return e.RightButton.Equals(MouseButtonState.Pressed);
        }
    }
}
