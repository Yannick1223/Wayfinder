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
        public LandscapeRenderer(int _row, int _column)
        {
            LandscapeRow = _row;
            LandscapeColumn = _column;

            //Default Settings
            LandscapeTileWidth = 32;
            LandscapeTileHeight = 32;

            OutlineThickness = 2;

            CalculateImageSize();

            Landscape = CreateLandscapeBitmap(ImageWidth, ImageHeight);
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

        public int OutlineThickness { get; private set; }

        private WriteableBitmap CreateLandscapeBitmap(int _width, int _height)
        {
            return BitmapFactory.New(_width, _height);
        }

        public void SetLandscapeSize(int _row, int _column)
        {
            LandscapeRow = _row;
            LandscapeColumn = _column;

            CalculateImageSize();
        }

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

        public void SetOutlineThickness(int _thickness)
        {
            OutlineThickness = _thickness;

            CalculateImageSize();
        }

        public void CalculateImageSize()
        {
            //All Tile Pixel + Border/Edge + between + Tile Pixel Border
            ImageWidth = LandscapeRow * LandscapeTileWidth + (LandscapeRow - 1) * OutlineThickness + 2 * OutlineThickness;
            ImageHeight = LandscapeColumn * LandscapeTileHeight + (LandscapeColumn - 1) * OutlineThickness + 2 * OutlineThickness;
        }


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

        public void DrawLandscapeOutline()
        {
            if(OutlineThickness > 0)
            {
                DrawBorder();
                DrawHorizontalOutlines();
                DrawVertícalOutlines();
            }
        }

        private void DrawBorder()
        {

            for(int thickness = 0; thickness < OutlineThickness; thickness++)
            {
                DrawRectangle(thickness, thickness, ImageWidth - thickness - 1, ImageHeight - thickness - 1, Colors.Black);
            }
        }

        private void DrawHorizontalOutlines()
        {
            using (Landscape.GetBitmapContext())
            {
                for(int y = 1; y < LandscapeColumn; y++)
                {
                    Point startPosition = new Point(0, y * LandscapeTileWidth + y * OutlineThickness);
                    Point endPosition = new Point(ImageWidth, y * LandscapeTileWidth + y * OutlineThickness + OutlineThickness);

                    DrawFillRectangle(startPosition, endPosition, Colors.Black);
                }
            }
        }

        private void DrawVertícalOutlines()
        {
            using (Landscape.GetBitmapContext())
            {
                for (int x = 1; x < LandscapeRow; x++)
                {
                    Point startPosition = new Point(x * LandscapeTileHeight + x * OutlineThickness, 0);
                    Point endPosition = new Point(x * LandscapeTileWidth + x * OutlineThickness + OutlineThickness, ImageWidth);

                    DrawFillRectangle(startPosition, endPosition, Colors.Black);
                }
            }
        }

        public void DrawColorTile(int _row, int _col, Color _color)
        {
            if(_row <= 0 || _col <= 0)
            {
                throw new ArgumentException("row or col can't be below 1");
            }

            using (Landscape.GetBitmapContext())
            {
                Point startTile = new Point(_row * LandscapeTileWidth + (_row - 1) * OutlineThickness + OutlineThickness - LandscapeTileWidth, _col * LandscapeTileHeight + (_col - 1) * OutlineThickness + OutlineThickness - LandscapeTileHeight);
                Point endTile = new Point(startTile.X + LandscapeTileWidth, startTile.Y + LandscapeTileHeight);

                DrawFillRectangle(startTile, endTile, _color);
            }
        }

        public void DrawImageAtTile(int _row, int _col, WriteableBitmap _image)
        {
            if (_row <= 0 || _col <= 0)
            {
                throw new ArgumentException("row or col can't be below 1");
            }

            using (Landscape.GetBitmapContext())
            {
                //Point startTile = new Point(_row * LandscapeTileWidth + (_row - 1) * OutlineThickness + OutlineThickness - LandscapeTileWidth, _col * LandscapeTileHeight + (_col - 1) * OutlineThickness + OutlineThickness - LandscapeTileHeight);
                //Point endTile = new Point(startTile.X + LandscapeTileWidth, startTile.Y + LandscapeTileHeight);

                //Point startTile = new Point(2, 2);

                //Landscape.Blit(startTile, _image, new Rect(0, 0, 32, 32), Colors.White, WriteableBitmapExtensions.BlendMode.None);
            }
        }

        public void DrawImageTileAtPosition(int _x1, int _y1, WriteableBitmap _image)
        {
            using (Landscape.GetBitmapContext())
            {
                Landscape.Blit(new Point(_x1, _y1), _image, new Rect(_x1, _y1, _image.Width, _image.Height), Color.FromArgb(255, 255, 255, 255), WriteableBitmapExtensions.BlendMode.None);
            }
        }
    }
}
