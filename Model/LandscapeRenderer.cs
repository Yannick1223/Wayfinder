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
        public LandscapeRenderer(int _row, int _column, int _tileWidth, int _tileHeight, int _borderThickness)
        {
            LandscapeRow = _row;
            LandscapeColumn = _column;
            LandscapeTileWidth = _tileWidth;
            LandscapeTileHeight = _tileHeight;
            BorderThickness = _borderThickness;
            CalculateImageSize();

            Landscape = CreateLandscapeBitmap(ImageWidth, ImageHeight);
            DrawLandscapeOutline();
        }

        public WriteableBitmap Landscape { get; private set; }
        //Overall size of the image = CalculateImageSize();
        public int ImageWidth { get; private set; }
        public int ImageHeight { get; private set; }

        //Size of the individual tiles (32px*32px)
        public int LandscapeTileWidth { get; private set; }
        public int LandscapeTileHeight { get; private set; }

        //Number of tiles
        public int LandscapeRow { get; private set; }
        public int LandscapeColumn { get; private set; }

        public int BorderThickness { get; private set; } //TODO: Change OutlineThickness to Border Thickness
        //public int OutlineThickness { get; private set; }

        private WriteableBitmap CreateLandscapeBitmap(int _width, int _height)
        {
            if (_width > MathF.Pow(2, 15) ||  _height > MathF.Pow(2, 15))
            {
                throw new Exception("To large Image");
            }
            return BitmapFactory.New(_width, _height);
        }

        public void SetLandscapeSize(int _row, int _column)
        {
            LandscapeRow = _row;
            LandscapeColumn = _column;

            CalculateImageSize();
        }

        public void DrawLandscapeOutline()
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

        //Remove
        public void SetTileSize(int _width, int _height)
        {
            if(_width < 0 || _height < 0)
            {
                throw new ArgumentException("Argument can't be below zero. Width:" +  _width + " Height: " + _height); //own exeption later
            }

            LandscapeTileWidth = _width;
            LandscapeTileHeight = _height;

            CalculateImageSize();
        }

        public void CalculateImageSize()
        {
            // All Tiles + 2 * Border
            ImageWidth = LandscapeRow * LandscapeTileWidth + 2 * BorderThickness;
            ImageHeight = LandscapeColumn * LandscapeTileHeight + 2 * BorderThickness;
        }

        public Point GetTileStartPosition(int _row, int _col)
        {
            if (_row <= 0 || _col <= 0)
            {
                throw new ArgumentException("row or col can't be below 1");
            }

            return new Point(_row * LandscapeTileWidth + BorderThickness - LandscapeTileWidth, _col * LandscapeTileHeight + BorderThickness - LandscapeTileHeight);
        }

        public Point GetTileEndPosition(int _row, int _col)
        {
            if (_row <= 0 || _col <= 0)
            {
                throw new ArgumentException("row or col can't be below 1");
            }

            Point startPosition = GetTileStartPosition(_row, _col);
            return new Point(startPosition.X + LandscapeTileWidth, startPosition.Y + LandscapeTileHeight);
        }

        public Point? GetTileFromPosition(int _x, int _y)
        {
            Point? result = null;
            int? tileRow = null;
            int? tileCol = null;
            if (!IsXPositionInsideBorder(_x) || !IsYPositionInsideBorder(_y)) return result;

            tileRow = (ImageWidth - 2 * BorderThickness - (ImageWidth - _x)) / LandscapeTileWidth;
            tileCol = (ImageHeight - 2 * BorderThickness -(ImageWidth - _y)) / LandscapeTileHeight;
            result = new Point((int)tileRow + 1, (int)tileCol + 1);

            return result;
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
        public void DrawPixel(int _x, int _y, Color _color)
        {
            using (Landscape.GetBitmapContext())
            {
                Landscape.SetPixel(_x, _y, _color);
            }
        }

        public void DrawLine(int _x1, int _y1, int _x2, int _y2, Color _color)
        {
            using (Landscape.GetBitmapContext())
            {
                Landscape.DrawLine(_x1, _y1, _x2, _y2, _color);
            }
        }

        public void DrawHorizontalLine(int _x1, int _x2, int _y1, Color _color)
        {
            DrawLine(_x1, _y1, _x2, _y1, _color);
        }

        public void DrawVerticalLine(int _y1, int _y2, int _x1, Color _color)
        {
            DrawLine(_x1, _y1, _x1, _y2, _color);
        }

        public void DrawRectangle(int _x1, int _y1, int _x2, int _y2, Color _color)
        {
            using (Landscape.GetBitmapContext())
            {
                Landscape.DrawRectangle(_x1, _y1, _x2, _y2, _color);
            }
        }

        public void DrawFillRectangle(int _x1, int _y1, int _x2, int _y2, Color _color)
        {
            using (Landscape.GetBitmapContext())
            {
                Landscape.FillRectangle(_x1, _y1, _x2, _y2, _color);
            }
        }

        public void DrawFillRectangle(Point from, Point to, Color _color)
        {
            DrawFillRectangle((int)from.X, (int)from.Y, (int)to.X, (int)to.Y, _color);
        }

        public void DrawColorAtTile(int _row, int _col, Color _color)
        {
            if(_row <= 0 || _col <= 0)
            {
                throw new ArgumentException("row or col can't be below 1");
            }

            using (Landscape.GetBitmapContext())
            {
                DrawFillRectangle(GetTileStartPosition(_row, _col), GetTileEndPosition(_row, _col), _color);
            }
        }

        // Optimize later/ or find a function
        public void DrawImageAtTile(int _row, int _col, WriteableBitmap _image)
        {
            if (_row <= 0 || _col <= 0)
            {
                throw new ArgumentException("row or col can't be below 1");
            }

            if (_image == null)
            {
                throw new Exception("Image is null");
            }

            if(_image.PixelHeight != LandscapeTileHeight || _image.PixelWidth != LandscapeTileWidth)
            {
                throw new Exception("Image has wrong size. Expected: " + LandscapeTileWidth + "*" + LandscapeTileHeight);
            }

            using (Landscape.GetBitmapContext())
            {
                Point startTile = GetTileStartPosition(_row, _col);

                Landscape.Blit(startTile, _image, new Rect(0, 0, _image.PixelWidth, _image.PixelHeight), Color.FromArgb(255, 255, 255, 255), WriteableBitmapExtensions.BlendMode.None);
            }
        }

        //Edit function or Remove
        public void DrawImageTileAtPosition(int _x1, int _y1, WriteableBitmap _image)
        {
            using (Landscape.GetBitmapContext())
            {
                Landscape.Blit(new Point(_x1, _y1), _image, new Rect(_x1, _y1, _image.Width, _image.Height), Color.FromArgb(255, 255, 255, 255), WriteableBitmapExtensions.BlendMode.None);
            }
        }
    }
}
