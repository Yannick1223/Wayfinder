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

namespace Wayfinder.Model
{
    public class LandscapeHandler
    {
        public LandscapeRenderer Renderer { get; private set; }
        public TileCollection Tiles { get; private set; }

        public LandscapeHandler(int _row, int _column, int _tileWidth, int _tileHeight, int _borderThickness)
        {
            Tiles = new TileCollection();
            Renderer = new LandscapeRenderer(_row, _column, _tileWidth, _tileHeight, _borderThickness);

            LoadDefaultTiles();
            GenerateWaterLandscape();
        }

        public void LoadDefaultTiles()
        {
            //Change later that there are more Images foreach Tile with different appearchances
            TileInformation[] defaultTiles = 
            {
                new TileInformation(TileType.Start, "Startpunkt", 1, new Uri(@"../Assets/startpoint.jpg", UriKind.Relative)),
                new TileInformation(TileType.End, "Endpunkt", 1, new Uri(@"../Assets/endpoint.jpg", UriKind.Relative)),
                new TileInformation(TileType.Land, "Land", 1, new Uri(@"../Assets/grass.jpg", UriKind.Relative)),
                new TileInformation(TileType.Desert, "Wüste", 2, new Uri(@"../Assets/sand.jpg", UriKind.Relative)),
                new TileInformation(TileType.Water, "Wasser", -1, new Uri(@"../Assets/water.jpg", UriKind.Relative)),
                new TileInformation(TileType.Forest, "Wald", 3, new Uri(@"../Assets/test2.jpg", UriKind.Relative))
            };

            Tiles.AddTiles(defaultTiles);
        }

        public ObservableCollection<TileInformation> GetObservableTiles()
        {
            return new ObservableCollection<TileInformation>(Tiles.Information);
        }

        public void GenerateWaterLandscape()
        {
            WriteableBitmap water = GetWriteableBitmap(TileType.Water);

            for (int x = 1; x < Renderer.Rows + 1; x++)
            {
                for (int y = 1; y < Renderer.Columns + 1; y++)
                {
                    Renderer.DrawImageAtTile(x, y, water);
                }
            }
        }

        public WriteableBitmap GetWriteableBitmap(TileType _type)
        {
            Uri? tilePath = Tiles.GetTileLocation(_type);
            if (tilePath == null) throw new Exception("No" + _type.ToString() + " Image found.");

            return BitmapFactory.FromContent(tilePath.OriginalString);
        }

        public void GenerateRandomLandscape()
        {
            WriteableBitmap water = GetWriteableBitmap(TileType.Water);
            WriteableBitmap land = GetWriteableBitmap(TileType.Land);
            WriteableBitmap sand = GetWriteableBitmap(TileType.Desert);

            WriteableBitmap[] tiles = { water, land, sand };

            Random rnd = new Random();

            for (int x = 1; x < Renderer.Rows + 1; x++)
            {
                for (int y = 1; y < Renderer.Columns + 1; y++)
                {
                    Renderer.DrawImageAtTile(x, y, tiles[rnd.Next(0, tiles.Length)]);
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
            Renderer.DrawImageAtTile(_row, _col, GetWriteableBitmap(_type));
        }

        public Point? GetTileFromPosition(Point _pos)
        {
            return GetTileFromPosition((int)_pos.X, (int)_pos.Y);
        }

        public Point? GetTileFromPosition(int _x, int _y)
        {
            return Renderer.PositionToTilePosition(_x, _y);
        }
    }
}
