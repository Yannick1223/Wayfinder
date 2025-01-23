using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection;
using Wayfinder.Model.Pathfinding;

namespace Wayfinder.Model
{
    public class LandscapeHandler
    {
        public LandscapeRenderer Renderer { get; private set; }
        public TileHandler Tiles { get; private set; }
        public PathfinderHandler Pathfinder { get; private set; }

        public Point? StartPointPosition { get; private set; }
        public bool IsStartpointSet { get; set; }
        public Point? EndPointPosition { get; private set; }
        public bool IsEndpointSet { get; set; }

        public LandscapeHandler(int _rows, int _column, int _tileWidth, int _tileHeight, int _borderThickness)
        {
            Renderer = new LandscapeRenderer(_rows, _column, _tileWidth, _tileHeight, _borderThickness);
            Tiles = new TileHandler(_rows, _column);
            Pathfinder = new PathfinderHandler();

            IsStartpointSet = false;
            IsEndpointSet = false;
        }

        public ObservableCollection<TileInformation> GetObservableTiles()
        {
            return new ObservableCollection<TileInformation>(Tiles.GetAllTilesInformation());
        }

        public void AddTile(TileInformation _tile)
        {
            Tiles.AddNewTile(_tile);
        }

        public void AddTiles(TileInformation[] _tiles)
        {
            Tiles.AddNewTiles(_tiles);
        }

        public void GenerateWaterLandscape()
        {
            WriteableBitmap water = Tiles.GetWriteableBitmap(TileType.Water);
            Tiles.SetAllTiles(TileType.Water);
            ResetStartPoint();
            ResetEndPoint();

            for (int x = 1; x < Renderer.Rows + 1; x++)
            {
                for (int y = 1; y < Renderer.Columns + 1; y++)
                {
                    Renderer.DrawImageAtTile(x, y, water);
                }
            }
        }

        public void GenerateRandomLandscape(TileType[] _tilesType)
        {
            WriteableBitmap[] tiles = Tiles.GetAllWriteableBitmaps(_tilesType);
            Random rnd = new Random();
            int rndnum;

            //Später beachten ob ein Tile das Starttile ist
            ResetStartPoint();
            ResetEndPoint();

            for (int x = 1; x < Renderer.Rows + 1; x++)
            {
                for (int y = 1; y < Renderer.Columns + 1; y++)
                {
                    rndnum = rnd.Next(0, tiles.Length);

                    Renderer.DrawImageAtTile(x, y, tiles[rndnum]);
                    Tiles.SetTile(x, y, _tilesType[rndnum]);
                }
            }
        }

        public void GenerateRandomColorLandscape()
        {
            Random rnd = new Random();

            for (int x = 1; x < Renderer.Rows + 1; x++)
            {
                for (int y = 1; y < Renderer.Columns + 1; y++)
                {
                    Renderer.DrawColorAtTile(x, y, System.Windows.Media.Color.FromRgb((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255)));
                }
            }
        }

        public void DrawTileAtPosition(int _row, int _col, TileType _type)
        {
            if (_type.Equals(Tiles.GetTileType(_row, _col))) return;

            if (_type.Equals(TileType.Start) && IsStartpointSet) return;
            if (_type.Equals(TileType.End) && IsEndpointSet) return;

            if (_type.Equals(TileType.Start) && !IsStartpointSet) 
            {
                IsStartpointSet = true;
                StartPointPosition = new Point(_row - 1, _col - 1);
            } 
            if (_type.Equals(TileType.End) && !IsEndpointSet)
            {
                IsEndpointSet = true;
                EndPointPosition = new Point(_row - 1, _col - 1);
            }

            if (Tiles.GetTileType(_row, _col).Equals(TileType.Start) && !_type.Equals(TileType.Start))
            {
                ResetStartPoint();
            }
            if (Tiles.GetTileType(_row, _col).Equals(TileType.End) && !_type.Equals(TileType.End))
            {
                ResetEndPoint();
            }

            Tiles.SetTile(_row, _col, _type);
            Renderer.DrawImageAtTile(_row, _col, Tiles.GetWriteableBitmap(_type));
        }

        public void DrawPath(List<Node>? _path)
        {
            if (_path == null) return;

            foreach (Node node in _path)
            {
                Renderer.DrawColorAtTile(node.X + 1, node.Y + 1, System.Windows.Media.Colors.Red);
            }
        }

        public void FindPath()
        {
            if(StartPointPosition == null || EndPointPosition == null) return;
            List<Node>? path = SearchPath(StartPointPosition.Value, EndPointPosition.Value);
            DrawPath(path);
        }

        private List<Node>? SearchPath(Point _from, Point _to)
        {
            return GetPath(_from, _to, Tiles.GetAllTileCost()); ;
        }

        private List<Node>? GetPath(Point _from, Point _to, int[,] _costs)
        {
            return Pathfinder.GetPath(_from, _to, _costs);
        }

        public Point? GetTileFromPosition(Point _pos)
        {
            return GetTileFromPosition((int)_pos.X, (int)_pos.Y);
        }

        public Point? GetTileFromPosition(int _x, int _y)
        {
            return Renderer.PositionToTilePosition(_x, _y);
        }

        private void ResetStartPoint()
        {
            IsStartpointSet = false;
            StartPointPosition = null;
        }

        private void ResetEndPoint()
        {
            IsEndpointSet = false;
            EndPointPosition = null;
        }
    }
}
