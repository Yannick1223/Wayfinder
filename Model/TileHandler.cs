using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Wayfinder.Model
{
    public class TileHandler
    {
        public TileCollection TilesList { get; private set; }
        public TileInformation[,] Tiles { get; private set; }

        public TileHandler(int _rows, int _columns)
        {
            TilesList = new TileCollection();
            Tiles = new TileInformation[_rows, _columns];
        }

        public TileHandler(int _rows, int _columns, TileInformation[] _tiles)
        {
            TilesList = new TileCollection();
            TilesList.AddTiles(_tiles);
            Tiles = new TileInformation[_rows, _columns];
        }

        public void AddNewTile(TileInformation _tile)
        {
            TilesList.AddTile(_tile);
        }

        public void AddNewTiles(TileInformation[] _tiles)
        {
            TilesList.AddTiles(_tiles);
        }

        public void SetTile(int _row, int _col, TileType _type)
        {
            TileInformation? info = TilesList.GetTileInformation(_type);

            if (info == null) throw new Exception("Tile not found.");
            Tiles[_row - 1, _col - 1] = info;
        }

        public void SetAllTiles(TileType _type)
        {
            TileInformation? info = TilesList.GetTileInformation(_type);

            if (info == null) throw new Exception("Tile not found.");

            for (int y = 1; y < Tiles.GetLength(1) + 1; y++)
            {
                for (int x = 1; x < Tiles.GetLength(0) + 1; x++)
                {
                    Tiles[x - 1, y - 1] = info;
                }
            }
        }

        public WriteableBitmap[] GetAllWriteableBitmaps(TileType[] _tiles)
        {
            WriteableBitmap[] result = new WriteableBitmap[_tiles.Length];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = GetWriteableBitmap(_tiles[i]);
            }

            return result;
        }

        public WriteableBitmap GetWriteableBitmap(TileType _type)
        {
            Uri? tilePath = TilesList.GetTileLocation(_type);
            if (tilePath == null) throw new Exception("No" + _type.ToString() + " Image found.");

            return BitmapFactory.FromContent(tilePath.OriginalString);
        }

        public int[,] GetAllTileCost()
        {
            int[,] result = new int[Tiles.GetLength(0), Tiles.GetLength(1)];

            for (int y = 0; y < Tiles.GetLength(1); y++)
            {
                for (int x = 0; x < Tiles.GetLength(0); x++)
                {
                    result[x, y] = Tiles[x, y].Cost;
                }
            }

            return result;
        }

        public TileType GetTileType(int _row, int _col)
        {
            return Tiles[_row - 1, _col - 1].Type;
        }

        public List<TileInformation> GetAllTilesInformation()
        {
            return TilesList.Information;
        }
    }
}
