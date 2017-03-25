using System;
using System.Collections.Generic;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Structure;
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
        public Image<Gray, byte> GetSinogram(Image<Gray,byte> inputImage, TomographConfiguration configuration)
        {
            int columnsCount = (int) (360/RadianToDegree(configuration.Alpha));

            float[,] brightnessArray = new float[columnsCount, configuration.DetectorsCount];           

            int width = inputImage.Width - 1;
            int height = inputImage.Height - 1;

            int radius = Math.Min(width/2,height/2);

            float angle = 0;
            int columnNumber = 0;
            
            while (columnNumber < columnsCount)
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
                    
                    brightnessArray[columnNumber,i] = GetBrightness(inputImage, emitterCoordinates, detectorCoordinates);
                }

                columnNumber++;
                angle += configuration.Alpha;
            }

            NormalizeArray(brightnessArray);

            return GetImageFromBrightnessArray(brightnessArray);
        }

        public IEnumerable<Image<Gray, byte>> GetOutputImagesFromSinogram(Image<Gray, byte> inputImage, Image<Gray, byte> sinogram, TomographConfiguration configuration)
        {
            var outputImages = new List<Image<Gray, byte>>();

            float[,] brightnessArray = new float[inputImage.Width, inputImage.Height];
            int[,] countArray = new int[inputImage.Width, inputImage.Height];

            int width = inputImage.Width - 1;
            int height = inputImage.Height - 1;

            int radius = Math.Min(width / 2, height / 2);

            float angle = 0;
            int columnNumber = 0;

            while (columnNumber < sinogram.Width)
            {
                var emitterCoordinates = new Coordinates()
                {
                    X = (int)(radius * Math.Cos(angle)) + width / 2,
                    Y = (int)(radius * Math.Sin(angle)) + height / 2
                };

                for (int i = 0; i < configuration.DetectorsCount; i++)
                {
                    var detectorCoordinates = new Coordinates()
                    {
                        X =
                            (int)
                                (radius *
                                 Math.Cos(angle + Math.PI - configuration.Phi / 2 +
                                          i * configuration.Phi / (configuration.DetectorsCount - 1))) + width / 2,
                        Y =
                            (int)
                                (radius *
                                 Math.Sin(angle + Math.PI - configuration.Phi / 2 +
                                          i * configuration.Phi / (configuration.DetectorsCount - 1))) + height / 2
                    };

                    float brightnessValue = sinogram.Data[i,columnNumber, 0];

                    DrawLine(brightnessArray, brightnessValue, countArray,emitterCoordinates, detectorCoordinates);
                }

                if((columnNumber+1) % (sinogram.Width/configuration.OutputImagesCount) == 0)
                {
                    var currentBrigthnessArray = (float[,]) brightnessArray.Clone();
                    DivideArray(currentBrigthnessArray, countArray);
                    NormalizeArray(currentBrigthnessArray);
                    outputImages.Add(GetImageFromBrightnessArray(currentBrigthnessArray));
                }

                columnNumber++;
                angle += configuration.Alpha;
            }
           
            return outputImages;
        }

        private void DivideArray(float[,] inputArray, int[,] countArray)
        {
            for (int i = 0; i < inputArray.GetLength(0); i++)
            {
                for (int j = 0; j < inputArray.GetLength(1); j++)
                {
                    if(countArray[i,j] > 1)
                        inputArray[i, j] /= countArray[i, j];
                }
            }
        }        

        private void NormalizeArray(float[,] inputArray)
        {
            float max = inputArray.Cast<float>().Max();
            float min = inputArray.Cast<float>().Min();

            for (int i = 0; i < inputArray.GetLength(0); i++)
            {
                for (int j = 0; j < inputArray.GetLength(1); j++)
                {
                    inputArray[i, j] = (inputArray[i, j] - min)*255/(max - min);                    
                }
            }
        }

        private Image<Gray, byte> GetImageFromBrightnessArray(float[,] brigthnessArray)
        {
            Image<Gray, byte> image = new Image<Gray, byte>(brigthnessArray.GetLength(0), brigthnessArray.GetLength(1));

            for (int i = 0; i < brigthnessArray.GetLength(1); i++)
            {
                for (int j = 0; j < brigthnessArray.GetLength(0); j++)
                {
                    image.Data[i, j, 0] = (byte) brigthnessArray[j, i];
                }
            }

            return image;
        }

        private float RadianToDegree(float angle)
        {
            return (float)(angle * (180.0 / Math.PI));
        }

        private float GetBrightness(Image<Gray, byte> inputImage,Coordinates emitterCoordinates, Coordinates detectorCoordinates)
        {
            float sum = 0;
            int count = 0;
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
                sum += inputImage.Data[y, x, 0];
                count++;
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
                    sum += inputImage.Data[y, x, 0];
                    count++;
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

                    sum += inputImage.Data[y, x, 0];
                    count++;
                }
            }

            return sum/count;
        }

        private void DrawLine(float[,] brightnessArray, float brightnessValue, int[,] countArray, Coordinates emitterCoordinates, Coordinates detectorCoordinates)
        {
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
                brightnessArray[x, y] += brightnessValue;
                countArray[x, y]++;
            }

            if (dx > dy)
            {
                ai = (dy - dx) * 2;
                bi = dy * 2;
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
                    brightnessArray[x, y] += brightnessValue;
                    countArray[x, y]++;
                }
            }
            else
            {
                ai = (dx - dy) * 2;
                bi = dx * 2;
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

                    brightnessArray[x, y] += brightnessValue;
                    countArray[x, y]++;
                }
            }
        }
    }
}
