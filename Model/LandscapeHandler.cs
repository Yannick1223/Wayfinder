using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;


namespace Wayfinder.Model
{
    public class LandscapeHandler
    {
        private LandscapeRenderer Renderer { get; set; }
        private TileHandler Tiles { get; set; }
        private PathfinderHandler Pathfinder { get; set; }

        private Point? StartPointPosition { get; set; }
        public bool IsStartpointSet { get; private set; }
        private Point? EndPointPosition { get; set; }
        public bool IsEndpointSet { get; private set; }


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
        public void GenerateLandscapeFromFile(int[,] _tileID)
        {
            TileType[] types = Enum.GetValues<TileType>();
            int typeLenght = types.Length;
            WriteableBitmap[] tiles = Tiles.GetAllWriteableBitmaps(types);

            ResetStartPoint();
            ResetEndPoint();

            for (int y = 1; y < _tileID.GetLength(1) + 1; y++)
            {
                for (int x = 1; x < _tileID.GetLength(0) + 1; x++)
                {
                    int tile = _tileID[x - 1, y - 1];
                    if (tile > typeLenght) continue;

                    TileType type = types[tile];

                    //If there are somehow more than 1 Start or Endpoints in the File
                    if(type.Equals(TileType.Start) && IsStartpointSet || type.Equals(TileType.End) && IsEndpointSet)
                    {
                        type = TileType.Water;
                        tile = (int)TileType.Water;
                    }

                    if (type.Equals(TileType.Start)) SetStartPoint(x, y);
                    if (type.Equals(TileType.End)) SetEndPoint(x, y);

                    Tiles.SetTile(x, y, type);
                    Renderer.DrawImageAtTile(x, y, tiles[tile]);
                }
            }
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

        public void GenerateMazeLandscape()
        {
            WriteableBitmap land = Tiles.GetWriteableBitmap(TileType.Land);
            WriteableBitmap start = Tiles.GetWriteableBitmap(TileType.Start);
            WriteableBitmap end = Tiles.GetWriteableBitmap(TileType.End);

            int mazeX = (Renderer.Rows % 2 == 0)? (int)(Renderer.Rows / 2) : (int)(Renderer.Rows / 2 + 1);
            int mazeY = (Renderer.Columns % 2 == 0)? (int)(Renderer.Columns / 2) : (int)(Renderer.Columns / 2 + 1);

            OriginShift shift = new OriginShift();
            Vector2D[,]? maze = shift.GenerateMaze(mazeX, mazeY, 25);

            if (maze == null) return;

            GenerateWaterLandscape();

            ResetStartPoint();
            ResetEndPoint();

            for (int y = 0; y < maze.GetLength(1); y++)
            {
                for (int x = 0; x < maze.GetLength(0); x++)
                {
                    int x1 = 2 * x + 1; 
                    int y1 = 2 * y + 1;

                    if (x1 > Renderer.Rows || y1 > Renderer.Columns) continue;

                    Renderer.DrawImageAtTile(x1, y1, land);
                    Tiles.SetTile(x1, y1, TileType.Land);

                    int x2 = 2 * x + 1 + (int)maze[x, y].X; 
                    int y2 = 2 * y + 1 + (int)maze[x, y].Y;

                    if (x2 > Renderer.Rows || y2 > Renderer.Columns) continue;

                    Renderer.DrawImageAtTile(x2, y2, land);
                    Tiles.SetTile(x2, y2, TileType.Land);
                }
            }

            //Set Startpoint to the begining of the Maze and the Endpoint to the end of the Maze
            SetStartPoint(1, 1);
            Renderer.DrawImageAtTile(1, 1, start);
            Tiles.SetTile(1, 1, TileType.Start);

            SetEndPoint(2 * mazeX - 1, 2 * mazeY - 1);
            Renderer.DrawImageAtTile(2 * mazeX - 1, 2 * mazeY - 1, end);
            Tiles.SetTile(2 * mazeX - 1, 2 * mazeY - 1, TileType.End);
        }

        public void GenerateSimplexNoiselandscape()
        {
            Random rnd = new Random();
            NoiseGenerator noise = new NoiseGenerator(rnd.Next(0, int.MaxValue - 1));

            WriteableBitmap water = Tiles.GetWriteableBitmap(TileType.Water);
            WriteableBitmap deepwater = Tiles.GetWriteableBitmap(TileType.DeepWater);
            WriteableBitmap land = Tiles.GetWriteableBitmap(TileType.Land);
            WriteableBitmap desert = Tiles.GetWriteableBitmap(TileType.Desert);
            WriteableBitmap tree = Tiles.GetWriteableBitmap(TileType.Forest);

            ResetStartPoint();
            ResetEndPoint();

            for (int x = 1; x < Renderer.Rows + 1; x++)
            {
                for (int y = 1; y < Renderer.Columns + 1; y++)
                {
                    double g = noise.GetValue(x, y, 0.05f) / 256;

                    if (g < 0.175)
                    {
                        Renderer.DrawImageAtTile(x, y, deepwater);
                        Tiles.SetTile(x, y, TileType.DeepWater);
                    }
                    else if(g < 0.65)
                    {
                        Renderer.DrawImageAtTile(x, y, water);
                        Tiles.SetTile(x, y, TileType.Water);
                    }
                    else if (g < 0.75)
                    {
                        Renderer.DrawImageAtTile(x, y, desert);
                        Tiles.SetTile(x, y, TileType.Desert);
                    }
                    else if (g < 0.89)
                    {
                        Renderer.DrawImageAtTile(x, y, land);
                        Tiles.SetTile(x, y, TileType.Land);
                    }
                    else if (g >= 0.89)
                    {
                        Renderer.DrawImageAtTile(x, y, tree);
                        Tiles.SetTile(x, y, TileType.Forest);
                    }
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
        public bool DrawTileAtPosition(int _row, int _col, TileType _type)
        {
            if (CheckTileRestrictions(_type, _row, _col)) return false;

            if (_type.Equals(TileType.Start) && !IsStartpointSet) SetStartPoint(_row, _col);
            if (_type.Equals(TileType.End) && !IsEndpointSet) SetEndPoint(_row, _col);

            if (Tiles.GetTileType(_row, _col).Equals(TileType.Start) && !_type.Equals(TileType.Start)) ResetStartPoint();
            if (Tiles.GetTileType(_row, _col).Equals(TileType.End) && !_type.Equals(TileType.End)) ResetEndPoint();


            Tiles.SetTile(_row, _col, _type);
            Renderer.DrawImageAtTile(_row, _col, Tiles.GetWriteableBitmap(_type));
            return true;
        }

        public void VisitTile(int _row1, int _col1, int _row2, int _col2, bool moveToStartPoint)
        {
            if (moveToStartPoint)
            {
                Renderer.DrawImageAtTile(_row1 + 1, _col1 + 1, Tiles.GetWriteableBitmap(TileType.Land));
                Renderer.DrawImageAtTile(_row2 + 1, _col2 + 1, Tiles.GetWriteableBitmap(TileType.Start));
            }
            else
            {
                Renderer.DrawImageAtTile(_row1 + 1, _col1 + 1, Tiles.GetWriteableBitmap(Tiles.Tiles[_row1, _col1].Type));
                Renderer.DrawImageAtTile(_row2 + 1, _col2 + 1, Tiles.GetWriteableBitmap(TileType.Start));
            }
        }

        public void ResetTile(int _row, int _col)
        {
            Renderer.DrawImageAtTile(_row + 1, _col + 1, Tiles.GetWriteableBitmap(Tiles.Tiles[_row, _col].Type));
        }

        public void ResetPath(List<Node>? _path)
        {
            if (_path == null) return;

            foreach(Node tile in _path)
            {
                Renderer.DrawImageAtTile(tile.X + 1, tile.Y + 1, Tiles.GetWriteableBitmap(Tiles.Tiles[tile.X, tile.Y].Type));
            }
        }


        //Pathfinding
        public List<Node>? SearchPath()
        {
            if (StartPointPosition == null || EndPointPosition == null) return null;
            return Pathfinder.GetPath(StartPointPosition.Value, EndPointPosition.Value, Tiles.GetAllTileCost());
        }

        public void ChangePathfinderalgorithm(Pathfinder _pathfinder)
        {
            Pathfinder.SetPathfinderAlgorithm(_pathfinder);
        }

        public void DebugDrawPath(List<Node>? _path, System.Windows.Media.Color _color)
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

        public TileHandler GetTileHandler()
        {
            return Tiles;
        }
    }
}
