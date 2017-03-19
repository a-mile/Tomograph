using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Tomograph.Library.Abstract;

namespace Tomograph.Library.SuperTomograph
{
    public class Coordinates
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class ConicalDetectorEmitterSystem : IEmitterDetectorSystem
    {
        public Bitmap GetSinogram(Bitmap inputBitmap, TomographConfiguration configuration)
        {
            int columnsCount = (int) (360/RadianToDegree(configuration.Alpha)) + 1;

            double[,] brightnessArray = new double[columnsCount, configuration.DetectorsCount];           

            int width = inputBitmap.Width - 1;
            int height = inputBitmap.Height - 1;

            int radius = Math.Min(width/2,height/2);

            double angle = 0;
            int columnNumber = 0;
            
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
                    
                    brightnessArray[columnNumber,i] = GetBrightness(inputBitmap, emitterCoordinates, detectorCoordinates);
                }

                columnNumber++;
                angle += configuration.Alpha;
            }

            NormalizeArray(brightnessArray);

            return GetBitmapFromBrightnessArray(brightnessArray);
        }

        public Bitmap[] GetOutputBitmapsFromSinogram(Bitmap sinogram, TomographConfiguration configuration)
        {
            throw new NotImplementedException();
        }

        private void NormalizeArray(double[,] inputArray)
        {            
            double max = inputArray.Cast<double>().Max();
            double min = inputArray.Cast<double>().Min();

            for (int i = 0; i < inputArray.GetLength(0); i++)
            {
                for (int j = 0; j < inputArray.GetLength(1); j++)
                {
                    inputArray[i, j] = (inputArray[i, j] - min)/(max - min);
                }
            }
        }

        private Bitmap GetBitmapFromBrightnessArray(double[,] brigthnessArray)
        {
            Bitmap bitmap = new Bitmap(brigthnessArray.GetLength(0), brigthnessArray.GetLength(1));

            for (int i = 0; i < brigthnessArray.GetLength(0); i++)
            {
                for (int j = 0; j < brigthnessArray.GetLength(1); j++)
                {
                    bitmap.SetPixel(i, j,
                        Color.FromArgb((int) (brigthnessArray[i, j]*255), (int) (brigthnessArray[i, j]*255),
                            (int) (brigthnessArray[i, j]*255)));
                }
            }

            return bitmap;
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
