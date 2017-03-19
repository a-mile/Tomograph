using System;
using System.Diagnostics;
using System.Drawing;
using Tomograph.Library.Abstract;

namespace Tomograph.Library.SuperTomograph
{
    public class Coordinates
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class SinogramGenerator : ISinogramGenerator
    {
        public Bitmap GetSinogram(Bitmap inputBitmap, TomographConfiguration configuration)
        {
            Bitmap sinogram = new Bitmap((int)(360/RadianToDegree(configuration.Alpha))+1, configuration.DetectorsCount);
            double[,] brightness = new double[(int)(360 / RadianToDegree(configuration.Alpha))+1, configuration.DetectorsCount];           

            int width = inputBitmap.Width - 1;
            int height = inputBitmap.Height - 1;

            int radius = Math.Min(width/2,height/2);

            double angle = 0;
            int column = 0;

            double maxBrightness = 0;
            double minBrightness = double.MaxValue;

            while (angle < Math.PI * 2)
            {
                var emitterCoordinates = new Coordinates()
                {
                    X = (int)(radius * Math.Cos(angle)) + width/2,
                    Y = (int)(radius * Math.Sin(angle)) + height/2
                };

                for (int i = 0; i < configuration.DetectorsCount; i++)
                {
                    var detectorCoordinates = new Coordinates()
                    {
                        X =
                            (int)
                                (radius*
                                 Math.Cos(angle + Math.PI - configuration.Phi/2 +
                                          i*configuration.Phi/(configuration.DetectorsCount - 1))) + width/2,
                        Y =
                            (int)
                                (radius*
                                 Math.Sin(angle + Math.PI - configuration.Phi/2 +
                                          i*configuration.Phi/(configuration.DetectorsCount - 1))) + height/2
                    };
                    
                    brightness[column,i] = GetBrightness(inputBitmap, emitterCoordinates, detectorCoordinates);

                    if (brightness[column, i] > maxBrightness)
                        maxBrightness = brightness[column, i];
                    if (brightness[column, i] < minBrightness)
                        minBrightness = brightness[column, i];
                }

                column++;
                angle += configuration.Alpha;
            }

            for (int i = 0; i < (int)(360 / RadianToDegree(configuration.Alpha)); i++)
            {
                for (int j = 0; j < configuration.DetectorsCount; j++)
                {
                    brightness[i, j] = (brightness[i, j] - minBrightness)/(maxBrightness - minBrightness);

                    sinogram.SetPixel(i, j,
                        Color.FromArgb((int) (brightness[i, j]*255), (int) (brightness[i, j]*255),
                            (int) (brightness[i, j]*255)));
                }
            }

            return sinogram;
        }

        private double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        private double GetBrightness(Bitmap inputBitmap,Coordinates emitterCoordinates, Coordinates detectorCoordinates)
        {
            double sum = 0;
            int xi, dx, yi, dy;
            int ai, bi, d;
            var x = emitterCoordinates.X;
            var y = emitterCoordinates.Y;

            if (emitterCoordinates.X < detectorCoordinates.X)
            {
                xi = 1;
                dx = detectorCoordinates.X - emitterCoordinates.X;
            }
            else
            {
                xi = -1;
                dx = emitterCoordinates.X - detectorCoordinates.X;
            }

            if (emitterCoordinates.Y < detectorCoordinates.Y)
            {
                yi = 1;
                dy = detectorCoordinates.Y - emitterCoordinates.Y;
            }
            else
            {
                yi = -1;
                dy = emitterCoordinates.Y - detectorCoordinates.Y;
                sum += inputBitmap.GetPixel(x, y).GetBrightness();
            }

            if (dx > dy)
            {
                ai = (dy - dx)*2;
                bi = dy*2;
                d = bi - dx;

                while (x != detectorCoordinates.X)
                {
                    if (d >= 0)
                    {
                        x += xi;
                        y += yi;
                        d += ai;
                    }
                    else
                    {
                        d += bi;
                        x += xi;
                    }
                    sum += inputBitmap.GetPixel(x, y).GetBrightness();
                }
            }
            else
            {
                ai = (dx - dy)*2;
                bi = dx*2;
                d = bi - dy;

                while (y != detectorCoordinates.Y)
                {
                    if (d >= 0)
                    {
                        x += xi;
                        y += yi;
                        d += ai;
                    }
                    else
                    {
                        d += bi;
                        y += yi;
                    }
                    sum += inputBitmap.GetPixel(x, y).GetBrightness();
                }
            }

            return sum;
        }
    }
}
