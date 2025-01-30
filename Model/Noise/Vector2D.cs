using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wayfinder.Model.Noise
{
    public struct Vector2D
    {
        public double X {  get; set; }
        public double Y { get; set; }

        public Vector2D()
        {
            X = 0;
            Y = 0;
        }

        public Vector2D(double _x, double _y)
        {
            X = _x;
            Y = _y;
        }

        public Vector2D Normalize()
        {
            double n = Math.Sqrt(X * X + Y * Y);
            if (n == 0) return new Vector2D(X, Y);

            return new Vector2D(X / n, Y / n);
        }

        public static Vector2D AngleToUnitCordinates(double _angle)
        {
            return new Vector2D(Math.Cos(_angle), Math.Sin(_angle));
        }

        public static double Abs(Vector2D a)
        { 
            return Math.Sqrt(a.X * a.X + a.Y * a.Y);
        }

        public static double Dot(Vector2D a, Vector2D b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        public static double Dot(Vector2D a, double angle)
        {
            return a.X * Math.Cos(angle) + a.Y * Math.Sin(angle);
        }

        public static Vector2D operator +(Vector2D a) => a;
        public static Vector2D operator -(Vector2D a) => -a;
        public static Vector2D operator +(Vector2D a, Vector2D b) => new Vector2D(a.X + b.X, a.Y + b.Y);
        public static Vector2D operator -(Vector2D a, Vector2D b) => new Vector2D(a.X - b.X, a.Y - b.Y);
        public static Vector2D operator *(double a, Vector2D b) => new Vector2D(a * b.X, a * b.Y);
        public static Vector2D operator /(double a, Vector2D b)
        {
            if (a == 0) throw new DivideByZeroException();
            return new Vector2D(b.X / a, b.Y / a);
        }
    }
}
