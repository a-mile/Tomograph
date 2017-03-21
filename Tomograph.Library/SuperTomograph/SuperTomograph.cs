using System.Collections.Generic;
using Emgu.CV;
using Emgu.CV.Structure;
using Tomograph.Library.Abstract;

namespace Tomograph.Library.SuperTomograph
{
    public class SuperTomograph
    {
        public TomographConfiguration Configuration { get; set; }

        public Image<Gray,byte> InputImage => _inputImage ?? (_inputImage = GetInputImage());
        public Image<Gray, byte> Sinogram {
            get { return _sinogram ?? (_sinogram = GetSinogram()); }
            set { _sinogram = value; }
        }
        public IEnumerable<Image<Gray, byte>> OutputImages => _outputImages ?? (_outputImages = GetOutputImages());
        
        private Image<Gray, byte> _inputImage;
        private readonly IImageLoader _bitmapLoader;

        private Image<Gray, byte> _sinogram;
        private readonly IEmitterDetectorSystem _emitterDetectorSystem;

        private IEnumerable<Image<Gray, byte>> _outputImages;

        private readonly IOutputImageFilter _outputImageFilter;

        public SuperTomograph(IImageLoader bitmapLoader, IEmitterDetectorSystem emitterDetectorSystem,
            IOutputImageFilter outputImageFilter)
        {
            _bitmapLoader = bitmapLoader;
            _emitterDetectorSystem = emitterDetectorSystem;
            _outputImageFilter = outputImageFilter;
            Configuration = new TomographConfiguration();
        }

        public void CleanTomograph()
        {
            _inputImage = null;
            _sinogram = null;
            _outputImages = null;
        }

        private Image<Gray, byte> GetInputImage()
        {
            return _bitmapLoader.LoadImage(Configuration.InputImagePath);
        }

        private Image<Gray, byte> GetSinogram()
        {
            return _emitterDetectorSystem.GetSinogram(InputImage,Configuration);
        }

        private IEnumerable<Image<Gray, byte>> GetOutputImages()
        {
            return _emitterDetectorSystem.GetOutputImagesFromSinogram(InputImage,Sinogram,Configuration);
        }

        private Image<Gray, byte> GetFilteredOutputImage(Image<Gray, byte> bitmap)
        {
            return _outputImageFilter.GetFilteredOutputImage(bitmap);
        }
    }
}
