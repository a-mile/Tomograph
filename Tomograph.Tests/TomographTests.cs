using System;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Structure;
using NUnit.Framework;
using Tomograph.Library.Infrastructure;
using Tomograph.Library.SuperTomograph;

namespace Tomograph.Tests
{
    public class TomographTests
    {
        [Test]
        public void LoadImage()
        {
            string imagePath = @"C:\Users\amileszko\Pictures\sample.png";           

            SuperTomograph tomograph = IoC.Container.GetInstance<SuperTomograph>();

            tomograph.Configuration.InputImagePath = imagePath;

            Image<Gray, byte> image = tomograph.InputImage;

            Assert.IsNotNull(image);
        }

        [Test]
        public void GetSinogram()
        {
            string imagePath = @"C:\Users\amileszko\Pictures\sample.png";

            SuperTomograph tomograph = IoC.Container.GetInstance<SuperTomograph>();

            TomographConfiguration configuration = new TomographConfiguration
            {
                InputImagePath = imagePath,
                Alpha = (float)Math.PI/180,
                Phi = (float)Math.PI/2,
                DetectorsCount = 360
            };

            tomograph.Configuration = configuration;

            Image<Gray, byte> image = tomograph.Sinogram;

            Assert.IsNotNull(image);

            tomograph.Sinogram.Save(@"C:\Users\amileszko\Pictures\sinogram.bmp");
        }

        [Test]
        public void GetOutputImage()
        {
            string imagePath = @"C:\Users\amileszko\Pictures\sample.png";

            SuperTomograph tomograph = IoC.Container.GetInstance<SuperTomograph>();

            TomographConfiguration configuration = new TomographConfiguration
            {
                InputImagePath = imagePath,
                Alpha = (float)Math.PI / 180,
                Phi = (float)Math.PI / 2,
                DetectorsCount = 360
            };

            tomograph.Configuration = configuration;

            var images = tomograph.OutputImages;

            Assert.NotNull(images.First());

            images.First().Save(@"C:\Users\amileszko\Pictures\output.bmp");
        }
    }
}
