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
        private LandscapeRenderer Renderer { get; set; }
        private TileHandler Tiles { get; set; }
        private PathfinderHandler Pathfinder { get; set; }

        private Point? StartPointPosition { get; set; }
        private bool IsStartpointSet { get; set; }
        private Point? EndPointPosition { get; set; }
        private bool IsEndpointSet { get; set; }


        public LandscapeHandler(int _rows, int _column, int _tileWidth, int _tileHeight, int _borderThickness)
        {
            Renderer = new LandscapeRenderer(_rows, _column, _tileWidth, _tileHeight, _borderThickness);
            Tiles = new TileHandler(_rows, _column);
            Pathfinder = new PathfinderHandler(new AStar());

            IsStartpointSet = false;
            IsEndpointSet = false;
        }

        public WriteableBitmap GetLandscape()
        {
            return Renderer.GetLandscape();
        }


        //Generate Landscapes
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

        [Obsolete]
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


        //Drawing
        //Refactor later
        public void DrawTileAtPosition(int _row, int _col, TileType _type)
        {
            if (CheckTileRestrictions(_type, _row, _col)) return;

            if (_type.Equals(TileType.Start) && !IsStartpointSet) SetStartPoint(_row, _col);
            if (_type.Equals(TileType.End) && !IsEndpointSet) SetEndPoint(_row, _col);

            if (Tiles.GetTileType(_row, _col).Equals(TileType.Start) && !_type.Equals(TileType.Start)) ResetStartPoint();
            if (Tiles.GetTileType(_row, _col).Equals(TileType.End) && !_type.Equals(TileType.End)) ResetEndPoint();

            Tiles.SetTile(_row, _col, _type);
            Renderer.DrawImageAtTile(_row, _col, Tiles.GetWriteableBitmap(_type));
        }


        //Pathfinding
        public void SearchPath()
        {
            if (StartPointPosition == null || EndPointPosition == null) return;
            List<Node>? path = Pathfinder.GetPath(StartPointPosition.Value, EndPointPosition.Value, Tiles.GetAllTileCost());

            DebugDrawPath(path, System.Windows.Media.Colors.Red);
        }

        public void ChangePathfinderalgorithm(Pathfinder _pathfinder)
        {
            Pathfinder.SetPathfinderAlgorithm(_pathfinder);
        }

        private void DebugDrawPath(List<Node>? _path, System.Windows.Media.Color _color)
        {
            if (_path == null) return;

            foreach (Node node in _path)
            {
                Renderer.DrawColorAtTile(node.X + 1, node.Y + 1, _color);
            }
        }


        //Add Tiles
        public void AddTile(TileInformation _tile)
        {
            Tiles.AddNewTile(_tile);
        }

        public void AddTiles(TileInformation[] _tiles)
        {
            Tiles.AddNewTiles(_tiles);
        }


        public ObservableCollection<TileInformation> GetObservableTiles()
        {
            return new ObservableCollection<TileInformation>(Tiles.GetAllTilesInformation());
        }
        

        public Point? GetTileFromPosition(Point _pos)
        {
            return GetTileFromPosition((int)_pos.X, (int)_pos.Y);
        }

        public Point? GetTileFromPosition(int _x, int _y)
        {
            return Renderer.PositionToTilePosition(_x, _y);
        }

        private bool CheckTileRestrictions(TileType _type, int _row, int _col)
        {
            return IsSelectedTileEqualCurrentTile(_type, _row, _col) || IsStartpointSelectedAndSet(_type) || IsEndpointSelectedAndSet(_type);
        }

        private bool IsSelectedTileEqualCurrentTile(TileType _type, int _row, int _col)
        {
            return _type.Equals(Tiles.GetTileType(_row, _col));
        }

        private bool IsStartpointSelectedAndSet(TileType _type)
        {
            return _type.Equals(TileType.Start) && IsStartpointSet;
        }

        private bool IsEndpointSelectedAndSet(TileType _type)
        {
            return _type.Equals(TileType.End) && IsEndpointSet;
        }
   

        private void SetStartPoint(int _row, int _col)
        {
            IsStartpointSet = true;
            StartPointPosition = new Point(_row - 1, _col - 1);
        }

        private void SetEndPoint(int _row, int _col)
        {
            IsEndpointSet = true;
            EndPointPosition = new Point(_row - 1, _col - 1);
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
