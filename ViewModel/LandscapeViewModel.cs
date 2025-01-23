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
            AddTiles(LoadDefaultTiles());
            Handler.GenerateWaterLandscape();
        }

        private TileInformation[] LoadDefaultTiles()
        {
            //Change later that there are more Images foreach Tile with different appearchances
            TileInformation[] defaultTiles =
            {
                new TileInformation(TileType.Start, "Startpunkt", 1, new Uri(@"../Assets/startpoint.jpg", UriKind.Relative)),
                new TileInformation(TileType.End, "Endpunkt", 1, new Uri(@"../Assets/endpoint.jpg", UriKind.Relative)),
                new TileInformation(TileType.Land, "Land", 1, new Uri(@"../Assets/grass.jpg", UriKind.Relative)),
                new TileInformation(TileType.Desert, "Wüste", 2, new Uri(@"../Assets/sand.jpg", UriKind.Relative)),
                new TileInformation(TileType.Water, "Wasser", 0, new Uri(@"../Assets/water.jpg", UriKind.Relative)),
                new TileInformation(TileType.Forest, "Wald", 3, new Uri(@"../Assets/test2.jpg", UriKind.Relative))
            };

            return defaultTiles;
        }

        public void AddTile(TileInformation _tile)
        {
            Handler.AddTile(_tile);
            UpdateObservableSelection();
        }

        public void AddTiles(TileInformation[] _tiles)
        {
            Handler.AddTiles(_tiles);
            UpdateObservableSelection();
        }

        public void UpdateObservableSelection()
        {
            ObservableTileInformation.Clear();
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
        public void OnCalculateAStarPath()
        {
            Handler.FindPath();
        }


        [RelayCommand]
        public void OnGenerateRandomLandscape()
        {
            Handler.GenerateRandomLandscape(new TileType[] { TileType.Land, TileType.Water, TileType.Desert });
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
