using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Linq;

namespace Wayfinder.Model
{
    public class OriginShift
    {
        private int Width;
        private int Height;

        public OriginShift()
        {

        }

        public Vector2D[,]? GenerateMaze(int _width, int _height, int _loop = 10)
        {
            if (_width < 2 || _height < 2) return null;

            Width = _width;
            Height = _height;

            Vector2D[,] result = InitMaze();
            Vector2D origin = new Vector2D(Width - 1, Height - 1);

            for (int i = 0; i < Width * Height * _loop; i++)
            {
                Vector2D dir = GetRandomDirection((int)origin.X, (int)origin.Y);

                result[(int)origin.X, (int)origin.Y] = dir;
                origin += dir;
            }

            return result;
        }

        private Vector2D[,] InitMaze()
        {
            Vector2D[,] result = new Vector2D[Width, Height];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (x == Width - 1 && y == Height - 1)
                    {
                        //Startposition
                        result[x, y] = new Vector2D(0, 0);
                        continue;
                    }

                    if (x + 1 < Width)
                    {
                        result[x, y] = new Vector2D(1, 0);
                    }
                    else
                    {
                        result[x, y] = new Vector2D(0, 1);
                    }
                }
            }

            return result;
        }

        private Vector2D GetRandomDirection(int x, int y)
        {
            Random rnd = new Random();
            Vector2D[] result = GetValidNeighbours(x, y).ToArray();

            return result[rnd.Next(0, result.Length)];
        }

        private List<Vector2D> GetValidNeighbours(int x, int y)
        {
            List<Vector2D> result = new List<Vector2D>();

            int[] dirX = { -1, 1, 0, 0 };
            int[] dirY = { 0, 0, -1, 1 };

            for (int i = 0; i < 4; i++)
            {
                int xPos = x + dirX[i];
                int yPos = y + dirY[i];

                if (IsValidPosition(xPos, yPos))
                {
                    result.Add(new Vector2D(dirX[i], dirY[i]));
                }
            }
            return result;
        }

        private bool IsValidPosition(int _x, int _y)
        {
            return IsXPositionValid(_x) && IsYPositionValid(_y);
        }

        private bool IsXPositionValid(int _x)
        {
            return _x >= 0 && _x < Width;
        }

        private bool IsYPositionValid(int _y)
        {
            return _y >= 0 && _y < Height;
        }
    }
}
