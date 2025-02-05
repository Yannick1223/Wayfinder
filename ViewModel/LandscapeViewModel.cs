using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
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
using Wayfinder.View;

namespace Wayfinder.ViewModel
{
    public partial class LandscapeViewModel: BaseViewModel
    {
        private LandscapeHandler Handler { get; set; }
        private Image Landscape { get; set; }

        public ObservableCollection<TileInformation> ObservableTileInformation { get; private set; }

        private ListBox SelectedItem { get; set; }

        [ObservableProperty]
        public float zoom;
        [ObservableProperty]
        public string zoomText;

        
        [ObservableProperty]
        public string pathFoundText;
        private int PathIndex{ get; set; }
        private List<Node>? Path { get; set; }

        private Slider ZoomSlider { get; set; }

        [ObservableProperty]
        public string selectedPathfindingAlgorithm;

        public LandscapeViewModel()
        {
            Title = "Wayfinder";
            SelectedPathfindingAlgorithm = "A Stern";
            PathFoundText = "0/0";
            PathIndex = 0;

            Handler = new LandscapeHandler(20, 20, 8, 8, 1);
            ObservableTileInformation = new ObservableCollection<TileInformation>();
            AddTiles(LoadDefaultTiles());
            Handler.GenerateWaterLandscape();
        }

        private TileInformation[] LoadDefaultTiles()
        {
            //Change later that there are more Images foreach Tile with different appearchances
            TileInformation[] defaultTiles =
            {
                new (TileType.Start, "Startpunkt", 1, new Uri(@"../Assets/startpoint.jpg", UriKind.Relative)),
                new (TileType.End, "Endpunkt", 1, new Uri(@"../Assets/endpoint.jpg", UriKind.Relative)),
                new (TileType.Land, "Land (1)", 1, new Uri(@"../Assets/grass.jpg", UriKind.Relative)),
                new (TileType.Desert, "Wüste (2)", 2, new Uri(@"../Assets/sand.jpg", UriKind.Relative)),
                new (TileType.Water, "Wasser (\u221E)", 0, new Uri(@"../Assets/water.jpg", UriKind.Relative)),
                new (TileType.DeepWater, "Wasser (\u221E)", 0, new Uri(@"../Assets/water_deep.jpg", UriKind.Relative)),
                new (TileType.Forest, "Baum (3)", 3, new Uri(@"../Assets/forest.jpg", UriKind.Relative)),
                new (TileType.Snow, "Schnee (2)", 2, new Uri(@"../Assets/snow.jpg", UriKind.Relative)),
                new (TileType.Bridge_Horizontal, "Brücke (1)", 1, new Uri(@"../Assets/bridge_horizontal.jpg", UriKind.Relative)),
                new (TileType.Bridge_Vertical, "Brücke (1)", 1, new Uri(@"../Assets/bridge_vertical.jpg", UriKind.Relative)),
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

        public void SetZoomSlider(Slider _slider)
        {
            ZoomSlider = _slider;
            SetZoom(ZoomSlider.Value);
        }


        private void SetLandscapeToImageControl()
        {
            Landscape.Source = Handler.GetLandscape();
        }

        [RelayCommand]
        public void OnCalculatePath()
        {
            ResetPath();
            Path = Handler.SearchPath();

            if (Path == null)
            {
                PathFoundText = "0/0";
            }
            else
            {
                PathFoundText = $"0/{Path.Count - 1}";
            }
        }

        [RelayCommand]
        public void OnNextPath()
        {
            if (Path == null) return;

            PathIndex = Math.Clamp(PathIndex + 1, 0, Path.Count - 1);
            PathFoundText = $"{PathIndex}/{Path.Count - 1}";

            bool moveAwayFromStartPoint = PathIndex == 1;

            Handler.VisitTile(Path[PathIndex - 1].X, Path[PathIndex - 1].Y, Path[PathIndex].X, Path[PathIndex].Y, moveAwayFromStartPoint);

            //Handler.VisitTile(Path[PathIndex - 1].X + 1, Path[PathIndex - 1].Y + 1, Path[PathIndex].X + 1, Path[PathIndex].Y + 1, moveToStartPoint);
        }

        [RelayCommand]
        public void OnPreviousPath()
        {
            if (Path == null) return;

            PathIndex = Math.Clamp(PathIndex - 1, 0, Path.Count - 1);
            PathFoundText = $"{PathIndex}/{Path.Count - 1}";

            //bool moveToStartPoint = PathIndex == 0;

            Handler.VisitTile(Path[PathIndex + 1].X, Path[PathIndex + 1].Y, Path[PathIndex].X, Path[PathIndex].Y, false);

            //Handler.VisitTile(Path[PathIndex + 1].X + 1, Path[PathIndex + 1].Y + 1, Path[PathIndex].X + 1, Path[PathIndex].Y + 1, moveToStartPoint);
        }

        private void ResetPath()
        {
            if(PathIndex != 0 && Path != null)
            {
                Handler.ResetTile(Path[0].X, Path[0].Y);
                Handler.ResetTile(Path[PathIndex].X, Path[PathIndex].Y);
            }

            PathIndex = 0;
            PathFoundText = "0/0";
            Path = null;
        }

        [RelayCommand]
        public void OnGenerateRandomLandscape()
        {
            List<TileType> tiles = Enum.GetValues<TileType>().ToList();
            tiles.RemoveAll(type => type.Equals(TileType.Start) || type.Equals(TileType.End) || type.Equals(TileType.Bridge_Horizontal) || type.Equals(TileType.Bridge_Vertical) || type.Equals(TileType.DeepWater));

            ResetPath();

            Handler.GenerateRandomLandscape(tiles.ToArray());
        }

        [RelayCommand]
        public void OnGenerateNewLandscape()
        {
            NewLandscape dialog = new NewLandscape();

            bool closed = dialog.ShowDialog().Equals(false);

            if (closed)
            {
                if(dialog.GeneratorVM.Rows.HasValue && dialog.GeneratorVM.Columns.HasValue)
                {
                    Handler = new LandscapeHandler(dialog.GeneratorVM.Rows.Value, dialog.GeneratorVM.Columns.Value, 8, 8, 1);
                    SetLandscapeToImageControl();
                    AddTiles(LoadDefaultTiles());
                    Handler.GenerateWaterLandscape();
                    ResetPath();
                }
                Console.WriteLine();
            }
        }

        [RelayCommand]
        public void OnGenerateWaterLandscape()
        {
            ResetPath();
            Handler.GenerateWaterLandscape();
        }

        [RelayCommand]
        public void OnGenerateNoiseLandscape()
        {
            ResetPath();
            Handler.GenerateSimplexNoiselandscape();
        }

        [RelayCommand]
        public void OnChangePathfindingAlgorithm(string _value)
        {
            if (_value == null) return;

            Pathfinder? finder = null;
            switch (_value.Trim().ToLower())
            {
                case "astar":
                    finder = new AStar();
                    SelectedPathfindingAlgorithm = "A Stern";
                    break;
                case "dijkstra":
                    finder = new Dijkstra();
                    SelectedPathfindingAlgorithm = "Dijkstra";
                    break;
                case "bfs":
                    finder = new BFS();
                    SelectedPathfindingAlgorithm = "BFS";
                    break;
                case "dfs":
                    finder = new DFS();
                    SelectedPathfindingAlgorithm = "DFS";
                    break;
                default:
                    throw new Exception($"Not implemented Pathfinding Algorithm: {_value}.");
            }
            ResetPath();
            Handler.ChangePathfinderalgorithm(finder);
        }

        [RelayCommand]
        public void OnSaveLandscape()
        {
            SaveFileDialog dialog = new SaveFileDialog();

            dialog.Filter = "Wayfinder File (*.wy)|*.wy";

            if (dialog.ShowDialog() == true && dialog.FileName != null)
            {
                SaveFileHandler.SaveFile(Handler.GetTileHandler().Tiles, dialog.FileName);
            }
        }

        [RelayCommand]
        public void OnLoadLandscape()
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "Wayfinder File (*.wy)|*.wy";

            if (dialog.ShowDialog() == true && dialog.FileName != null)
            {
                int[,]? tiles = LoadFileHandler.LoadFile(dialog.FileName);

                if (tiles != null)
                {
                    Handler = new LandscapeHandler(tiles.GetLength(0), tiles.GetLength(1), 8, 8, 1);
                    SetLandscapeToImageControl();
                    AddTiles(LoadDefaultTiles());

                    Handler.GenerateLandscapeFromFile(tiles);

                    Path = null;
                    ResetPath();
                }
            }
        }

        [RelayCommand]
        public void OnHoverOverImage(MouseEventArgs e)
        {
            DrawTile(e);
        }

        [RelayCommand]
        public void OnZoomInLandscape()
        {
            SetZoom(ZoomSlider.Value + 0.25);
            UpdateZoomSlider();
        }

        [RelayCommand]
        public void OnZoomOutLandscape()
        {
            SetZoom(ZoomSlider.Value - 0.25);
            UpdateZoomSlider();
        }

        [RelayCommand]
        public void OnZoomWithSlider()
        {
            SetZoom(ZoomSlider.Value);
        }

        private void SetZoom(double _zoom)
        {
            Zoom = (float)Math.Clamp(_zoom, ZoomSlider.Minimum, ZoomSlider.Maximum);
            ZoomText = $"{(int)(Zoom * 100)}%";
        }

        private void UpdateZoomSlider()
        {
            ZoomSlider.Value = Zoom;
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
                bool drawed = Handler.DrawTileAtPosition((int)tilePos.Value.X, (int)tilePos.Value.Y, _type.Value);

                if (drawed)
                {
                    ResetPath();
                }
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
