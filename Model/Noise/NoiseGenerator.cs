using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Wayfinder.Model
{
    public class NoiseGenerator
    {
        public NoiseGenerator(int seed)
        {
            SimplexNoise.Noise.Seed = seed;
        }

        public float FractalBrownianMotion(int x, int y, int Octaves)
        {
            float result = 0;
            float amplitude = 1;
            float frequency = 0.05f;

            for (int octave = 0; octave < Octaves; octave++)
            {
                float n = amplitude * GetValue(x, y, frequency);
                result += n;

                amplitude *= 0.5f;
                frequency *= 1.5f; 
            }

            return result;
        }

        public float GetValue(int x, int y, float frequency)
        {
            return SimplexNoise.Noise.CalcPixel2D(x, y, frequency);
        }

        //obsolete
        /*
         
        public int Seed;

        public Dictionary<(int x, int y), int> Angles;

        public int Col;
        public int Row;

        public int ChunkSize;

        public PerlinNoise(int seed)
        {
            Seed = seed;
            ChunkSize = 256;
            SetAngles(ChunkSize, ChunkSize, Seed);
        }
         
          
        public double Perlin(int x, int y)
        {
            double X = x % (ChunkSize - 1); // % ChunkSize?
            double Y = y % (ChunkSize - 1);

            double xf = X - Math.Floor(X);
            double yf = Y - Math.Floor(Y);

            Console.WriteLine();

            Vector2D normal = new Vector2D(X, Y).Normalize();

            double topLeftNoise = Vector2D.Dot(normal,Angles[((int)X, (int)Y)]);
            double topRightNoise = Vector2D.Dot(normal, Angles[((int)X + 1, (int)Y)]);
            double bottomLeftNoise = Vector2D.Dot(normal, Angles[((int)X, (int)Y + 1)]);
            double bottomRightNoise = Vector2D.Dot(normal, Angles[((int)X + 1, (int)Y + 1)]);

            double u = Fade(xf);
            double v = Fade(yf);

            double result = Lerp(u, Lerp(v, bottomLeftNoise, topLeftNoise), Lerp(v, bottomRightNoise, topRightNoise));

            
            return result;
        }

        private double Lerp(double t, double a, double b)
        {
            return a + t * (b - a);
        }

        private double Fade(double x)
        {
            return ((6 * x - 15) * x + 10) * x * x * x;
        }

        public void SetAngles(int row, int col, int seed)
        {
            Random rnd = new Random(seed);
            Angles = new Dictionary<(int x, int y), int>();

            for (int x = 0; x < MathF.Max(row, col); x++)
            {
                for (int y = 0; y < MathF.Max(row, col); y++)
                {
                    Angles.Add((x, y), rnd.Next(0, 360));
                }
            }

            *//*for (int offset = 0; offset < MathF.Max(row, col); offset++)
            {
                for (int y = 0; y <= offset; y++)
                {
                    if (y >= row || y >= col)
                    {
                        rnd.Next(0, 360);
                        continue;
                    }
                    Angles.Add((offset, y), rnd.Next(0, 360));
                }

                for (int x = offset - 1; x >= 0; x--)
                {
                    if (offset >= row ||offset >= col)
                    {
                        rnd.Next(0, 360);
                        continue;
                    }
                    Angles.Add((x, offset), rnd.Next(0, 360));
                }
            }
        }*/
    }
}
