using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Net.NetworkInformation;

namespace Wayfinder.Model
{
    public class LandscapeRenderer
    {
        public WriteableBitmap Landscape { get; private set; }
        public int ImageWidth { get; private set; }
        public int ImageHeight { get; private set; }

        public int TileWidth { get; private set; }
        public int TileHeight { get; private set; }

        public int Rows { get; private set; }
        public int Columns { get; private set; }

        public int BorderThickness { get; private set; }

        public LandscapeRenderer(int _rows, int _columns, int _tileWidth, int _tileHeight, int _borderThickness)
        {
            Rows = _rows;
            Columns = _columns;
            TileWidth = _tileWidth;
            TileHeight = _tileHeight;
            BorderThickness = _borderThickness;
            CalculateImageSize();

            Landscape = CreateLandscapeBitmap(ImageWidth, ImageHeight);
            DrawLandscapeOutline();
        }

        private WriteableBitmap CreateLandscapeBitmap(int _width, int _height)
        {
            if (_width > (MathF.Pow(2, 15) - 1) ||  _height > (MathF.Pow(2, 15) - 1)) throw new Exception("To large Image");

            return BitmapFactory.New(_width, _height);
        }

        public void CalculateImageSize()
        {
            ImageWidth = Rows * TileWidth + 2 * BorderThickness;
            ImageHeight = Columns * TileHeight + 2 * BorderThickness;
        }

        private void DrawLandscapeOutline()
        {
            if (BorderThickness > 0)
            {
                DrawBorder();
            }
        }

        private void DrawBorder()
        {
            for (int thickness = 0; thickness < BorderThickness; thickness++)
            {
                DrawRectangle(thickness, thickness, ImageWidth - thickness - 1, ImageHeight - thickness - 1, Colors.Black);
            }
        }

        public Point GetStartPositionFromTile(int _row, int _col)
        {
            if (_row <= 0 || _col <= 0) throw new ArgumentException("row or col can't be below 1");

            return new Point(_row * TileWidth + BorderThickness - TileWidth, _col * TileHeight + BorderThickness - TileHeight);
        }

        public Point GetEndPositionFromTIle(int _row, int _col)
        {
            if (_row <= 0 || _col <= 0) throw new ArgumentException("row or col can't be below 1");

            return new Point(_row * TileWidth + BorderThickness, _col * TileHeight + BorderThickness);
        }

        public Point? PositionToTilePosition(int _x, int _y)
        {
            if (!IsPositionInsideBorder(_x, _y)) return null;

            return new Point(XPositionToTilePosition(_x), YPositionToTilePosition(_y));
        }

        private int XPositionToTilePosition(int _x)
        {
            return (ImageWidth - 2 * BorderThickness - (ImageWidth - _x)) / TileWidth + 1;
        }

        private int YPositionToTilePosition(int _y)
        {
            return (ImageHeight - 2 * BorderThickness - (ImageHeight - _y)) / TileHeight + 1;
        }

        private bool IsPositionInsideBorder(int _x, int _y)
        {
            return IsXPositionInsideBorder(_x) && IsYPositionInsideBorder(_y);
        }

        private bool IsXPositionInsideBorder(int _x)
        {
            return _x > BorderThickness && _x <= ImageWidth - BorderThickness;
        }

        private bool IsYPositionInsideBorder(int _y)
        { 
            return _y > BorderThickness && _y <= ImageHeight - BorderThickness;
        }

        //Drawing Functions
        public void DrawColorAtTile(int _row, int _col, Color _color)
        {
            if (_row <= 0 || _col <= 0) throw new ArgumentException("row or col can't be below 1");

            using (Landscape.GetBitmapContext())
            {
                DrawFillRectangle(GetStartPositionFromTile(_row, _col), GetEndPositionFromTIle(_row, _col), _color);
            }
        }

        public void DrawImageAtTile(int _row, int _col, WriteableBitmap _image)
        {
            if (_row <= 0 || _col <= 0) throw new ArgumentException("row or col can't be below 1");
            if (_image == null) throw new Exception("Image is null");
            if (_image.PixelHeight != TileHeight || _image.PixelWidth != TileWidth) throw new Exception("Image has wrong size. Expected: " + TileWidth + "*" + TileHeight);

            using (Landscape.GetBitmapContext())
            {
                Point startTile = GetStartPositionFromTile(_row, _col);

                Landscape.Blit(startTile, _image, new Rect(0, 0, _image.PixelWidth, _image.PixelHeight), Color.FromArgb(255, 255, 255, 255), WriteableBitmapExtensions.BlendMode.None);
            }
        }

        private void DrawPixel(int _x, int _y, Color _color)
        {
            using (Landscape.GetBitmapContext())
            {
                Landscape.SetPixel(_x, _y, _color);
            }
        }

        private void DrawLine(int _x1, int _y1, int _x2, int _y2, Color _color)
        {
            using (Landscape.GetBitmapContext())
            {
                Landscape.DrawLine(_x1, _y1, _x2, _y2, _color);
            }
        }

        private void DrawHorizontalLine(int _x1, int _x2, int _y1, Color _color)
        {
            DrawLine(_x1, _y1, _x2, _y1, _color);
        }

        private void DrawVerticalLine(int _y1, int _y2, int _x1, Color _color)
        {
            DrawLine(_x1, _y1, _x1, _y2, _color);
        }

        private void DrawRectangle(int _x1, int _y1, int _x2, int _y2, Color _color)
        {
            using (Landscape.GetBitmapContext())
            {
                Landscape.DrawRectangle(_x1, _y1, _x2, _y2, _color);
            }
        }

        private void DrawFillRectangle(int _x1, int _y1, int _x2, int _y2, Color _color)
        {
            using (Landscape.GetBitmapContext())
            {
                Landscape.FillRectangle(_x1, _y1, _x2, _y2, _color);
            }
        }

        private void DrawFillRectangle(Point _from, Point _to, Color _color)
        {
            DrawFillRectangle((int)_from.X, (int)_from.Y, (int)_to.X, (int)_to.Y, _color);
        }
    }
}
