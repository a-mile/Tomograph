using System;
using System.IO;
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
        private readonly string _projectDirectory;
        private readonly string _samplePath;

        public TomographTests()
        {
            _projectDirectory =
                Directory.GetParent(AppDomain.CurrentDomain.SetupInformation.ApplicationBase).Parent?.FullName + @"\images\";
            _samplePath = _projectDirectory + "sample.png";
        }

        [Test]
        public void LoadImage()
        {          
            SuperTomograph tomograph = IoC.Container.GetInstance<SuperTomograph>();

            tomograph.Configuration.InputImagePath = _samplePath;

            Image<Gray, byte> image = tomograph.InputImage;

            Assert.IsNotNull(image);
        }

        [Test]
        public void GetSinogram()
        { 
            SuperTomograph tomograph = IoC.Container.GetInstance<SuperTomograph>();

            TomographConfiguration configuration = new TomographConfiguration
            {
                InputImagePath = _samplePath,
                Alpha = (float)Math.PI/180,
                Phi = (float)Math.PI/2,
                DetectorsCount = 360
            };

            tomograph.Configuration = configuration;

            Image<Gray, byte> image = tomograph.Sinogram;

            Assert.IsNotNull(image);

            image.Save(_projectDirectory + "sinogram.bmp");
        }

        [Test]
        public void GetOutputImage()
        {
            SuperTomograph tomograph = IoC.Container.GetInstance<SuperTomograph>();

            TomographConfiguration configuration = new TomographConfiguration
            {
                InputImagePath = _samplePath,
                Alpha = (float)Math.PI / 180,
                Phi = (float)Math.PI / 2,
                DetectorsCount = 360,
                Filter = false,
                OutputImagesCount = 10
            };

            tomograph.Configuration = configuration;

            var images = tomograph.OutputImages.ToList();

            Assert.IsNotEmpty(images);

            for(int i=0; i<images.Count;i++)
            {
                images[i].Save(_projectDirectory + @"outputImages\" + $"output{i+1}.bmp");
            }            
        }
    }
}
