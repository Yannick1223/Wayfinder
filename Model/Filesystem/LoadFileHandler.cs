﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace Wayfinder.Model
{
    public class LoadFileHandler
    {
        public static int[,]? LoadFile(string _path)
        {
            string[]? lines = GetLines(_path);

            if (_path.Split('.').Last().ToLower() != "wy" || lines == null)
            {
                MessageBox.Show("Fehler beim lesen der Datei.", "Fehler");
                return null;
            }

            int? width = FromFileGetWidth(lines[0]);
            int? height = FromFileGetHeight(lines[0]);

            if (width == null || height == null || width < 1 || height < 1)
            {
                MessageBox.Show("Fehler beim lesen der Datei.", "Fehler");
                return null;
            }

            lines = lines.Skip(1).ToArray();

            int[,] tiles = new int[width.Value, height.Value];

            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                int? x = FromFileGetX(line);
                int? y = FromFileGetY(line);
                int? id = FromFileGetTileID(line);

                if (x == null || y == null || x < 0 || y < 0 || id == null)
                {
                    MessageBox.Show("Fehler beim lesen der Datei.", "Fehler");
                    return null;
                }

                tiles[x.Value, y.Value] = id.Value;
            }

            return tiles;
        }

        private static string[]? GetLines(string _path)
        {
            if (!System.IO.File.Exists(_path)) return null;

            return System.IO.File.ReadAllText(_path).Split("\n");
        }

        private static int? FromFileGetWidth(string _line)
        {
            int? width = null;

            if (int.TryParse(_line.Split('|')[0], out int _width))
            {
                width = _width;
            }

            return width;
        }

        private static int? FromFileGetHeight(string _line)
        {
            int? height = null;

            if (int.TryParse(_line.Split('|')[1], out int _height))
            {
                height = _height;
            }

            return height;
        }

        private static int? FromFileGetX(string _line)
        {
            int? x = null;

            if (int.TryParse(_line.Split(':')[0].Split(',')[0].Trim('('), out int xPos))
            {
                x = xPos - 1;
            }

            return x;
        }

        private static int? FromFileGetY(string _line)
        {
            int? y = null;

            if (int.TryParse(_line.Split(':')[0].Split(',')[1].Trim(')'), out int yPos))
            {
                y = yPos - 1;
            }

            return y;
        }

        private static int? FromFileGetTileID(string _line)
        {
            int? id = null;

            if (int.TryParse(_line.Split(':')[1], out int ID))
            {
                id = ID;
            }

            return id;
        }
    }
}
