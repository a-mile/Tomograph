using System;
using System.Drawing;
using NUnit.Framework;
using Tomograph.Library.Infrastructure;
using Tomograph.Library.SuperTomograph;

namespace Tomograph.Tests
{
    public class TomographTests
    {
        [Test]
        public void LoadBitmap()
        {
            string bitmapPath = @"C:\Users\amileszko\Pictures\sample.bmp";

            SuperTomograph tomograph = IoC.Container.GetInstance<SuperTomograph>();

            tomograph.Configuration.InputBitmapPath = bitmapPath;

            Bitmap bitmap = tomograph.InputBitmap;

            Assert.IsNotNull(bitmap);
        }

        [Test]
        public void GetSinogram()
        {
            string bitmapPath = @"C:\Users\amileszko\Pictures\sample.png";

            SuperTomograph tomograph = IoC.Container.GetInstance<SuperTomograph>();

            TomographConfiguration configuration = new TomographConfiguration
            {
                InputBitmapPath = bitmapPath,
                Alpha = Math.PI/180,
                Phi = Math.PI/2,
                DetectorsCount = 100
            };

            tomograph.Configuration = configuration;

            Bitmap bitmap = tomograph.Sinogram;

            Assert.IsNotNull(bitmap);

            tomograph.Sinogram.Save(@"C:\Users\amileszko\Pictures\sinogram.bmp");
        }
    }
}
