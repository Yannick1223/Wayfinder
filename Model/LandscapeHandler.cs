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
        public LandscapeHandler(int _row, int _column, int _tileWidth, int _tileHeight, int _borderThickness)
        {
            Tiles = new TileCollection();
            Renderer = new LandscapeRenderer(_row, _column, _tileWidth, _tileHeight, _borderThickness);

            LoadPredefinedTiles();
            //GenerateWaterLandscape();
            GenerateRandomLandscape();
            //GenerateRandomColorLandscape();
        }

        public LandscapeRenderer Renderer { get; private set; }
        public TileCollection Tiles { get; private set; }

        //public WriteableBitmap TileSprites { get; private set; }

        public void LoadPredefinedTiles()
        {
            //Change later that there are more Images foreach Tile with different appearchances
            Tiles.AddNewTile(new TileInformation(TileType.Land, "Land", 1, new Uri(@"../Assets/grass.jpg", UriKind.Relative)));
            Tiles.AddNewTile(new TileInformation(TileType.Water, "Wasser", -1, new Uri(@"../Assets/water.jpg", UriKind.Relative)));
            Tiles.AddNewTile(new TileInformation(TileType.Forest, "Wald", 3, new Uri(@"../Assets/test2.jpg", UriKind.Relative)));
            Tiles.AddNewTile(new TileInformation(TileType.Desert, "Wüste", 2, new Uri(@"../Assets/sand.jpg", UriKind.Relative)));
            Tiles.AddNewTile(new TileInformation(TileType.Start, "Startpunkt", 1, new Uri(@"../Assets/test2.jpg", UriKind.Relative)));
            Tiles.AddNewTile(new TileInformation(TileType.End, "Endpunkt", 1, new Uri(@"../Assets/test2.jpg", UriKind.Relative)));
        }

        public ObservableCollection<TileInformation> GetObservableTiles()
        {
            return new ObservableCollection<TileInformation>(Tiles.Information);
        }

        public void GenerateWaterLandscape()
        {
            Uri? waterPath = Tiles.GetTileLocation(TileType.Water);
            if (waterPath == null) throw new Exception("No Waterimage found.");

            WriteableBitmap water = BitmapFactory.FromContent(waterPath.OriginalString);

            for (int x = 1; x < Renderer.LandscapeRow + 1; x++)
            {
                for (int y = 1; y < Renderer.LandscapeColumn + 1; y++)
                {
                    Renderer.DrawImageAtTile(x, y, water);
                }
            }
        }

        public void GenerateRandomLandscape()
        {
            Uri? waterPath = Tiles.GetTileLocation(TileType.Water);
            if (waterPath == null) throw new Exception("No Waterimage found.");
            Uri? landPath = Tiles.GetTileLocation(TileType.Land);
            if (landPath == null) throw new Exception("No Landimage found.");
            Uri? sandPath = Tiles.GetTileLocation(TileType.Desert);
            if (sandPath == null) throw new Exception("No Sandimage found.");

            WriteableBitmap water = BitmapFactory.FromContent(waterPath.OriginalString);
            WriteableBitmap land = BitmapFactory.FromContent(landPath.OriginalString);
            WriteableBitmap sand = BitmapFactory.FromContent(sandPath.OriginalString);

            WriteableBitmap[] tiles = { water, land, sand };

            Random rnd = new Random();

            for (int x = 1; x < Renderer.LandscapeRow + 1; x++)
            {
                for (int y = 1; y < Renderer.LandscapeColumn + 1; y++)
                {
                    Renderer.DrawImageAtTile(x, y, tiles[rnd.Next(0, tiles.Length)]);
                }
            }
        }

        public void GenerateRandomColorLandscape()
        {
            Random rnd = new Random();

            for (int x = 1; x < Renderer.LandscapeRow + 1; x++)
            {
                for (int y = 1; y < Renderer.LandscapeColumn + 1; y++)
                {
                    Renderer.DrawColorAtTile(x, y, System.Windows.Media.Color.FromRgb((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255)));
                }
            }
        }

        public void DrawTileAtPosition(int _row, int _col, TileType _type)
        {
            Uri? tile = Tiles.GetTileLocation(_type);
            if (tile == null) throw new Exception("No Tile found.");

            WriteableBitmap tileWB = BitmapFactory.FromContent(tile.OriginalString);
            Renderer.DrawImageAtTile(_row, _col, tileWB);


        }

        public Point? GetTileFromPosition(int _x, int _y)
        {
            return Renderer.GetTileFromPosition(_x, _y);
        }
    }
}
