using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wayfinder.Model
{
    public class SaveFileHandler
    {
        public static void SaveFile(TileInformation[,] _tiles, string _path)
        {
            if (_tiles == null || _path == null) return;

            string file = $"{_tiles.GetLength(0)}|{_tiles.GetLength(1)}\n";

            for (int y = 0; y < _tiles.GetLength(1); y++)
            {
                for (int x = 0; x < _tiles.GetLength(0); x++)
                {
                    if (_tiles[x, y] != null)
                    {
                        file += $"({x + 1},{y + 1}): {(int)_tiles[x, y].Type}\n";
                    }
                }
            }

            System.IO.File.WriteAllText(_path, file);
        }
    }
}
