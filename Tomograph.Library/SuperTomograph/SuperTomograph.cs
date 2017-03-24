using System.Collections.Generic;
using Emgu.CV;
using Emgu.CV.Structure;
using Tomograph.Library.Abstract;

namespace Tomograph.Library.SuperTomograph
{
    public class SuperTomograph
    {
        public TomographConfiguration Configuration { get; set; }

        public Image<Gray, byte> InputImage {
            get { return _inputImage ?? (_inputImage = GetInputImage()); }
            set { _inputImage = value; }
        }

        public Image<Gray, byte> Sinogram {
            get { return _sinogram ?? (_sinogram = GetSinogram()); }
            set { _sinogram = value; }
        }

        public Image<Gray, byte> FilteredSinogram
        {
            get { return _filteredSinogram ?? (_filteredSinogram = GetFilteredSinogram()); }
            set { _filteredSinogram = value; }
        } 

        public IEnumerable<Image<Gray, byte>> OutputImages => _outputImages ?? (_outputImages = GetOutputImages());
        
        private Image<Gray, byte> _inputImage;
        private readonly IImageLoader _bitmapLoader;

        private Image<Gray, byte> _sinogram;
        private IEnumerable<Image<Gray, byte>> _outputImages;
        private readonly IEmitterDetectorSystem _emitterDetectorSystem;

        private Image<Gray, byte> _filteredSinogram;
        private readonly ISinogramFilter _sinogramFilter;
        
        public SuperTomograph(IImageLoader bitmapLoader, IEmitterDetectorSystem emitterDetectorSystem,
            ISinogramFilter sinogramFilter)
        {
            _bitmapLoader = bitmapLoader;
            _emitterDetectorSystem = emitterDetectorSystem;
            _sinogramFilter = sinogramFilter;
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
            return _emitterDetectorSystem.GetOutputImagesFromSinogram(InputImage,
                Configuration.Filter ? FilteredSinogram : Sinogram, Configuration);
        }

        private Image<Gray, byte> GetFilteredSinogram()
        {
            return _sinogramFilter.GetFilteredSinogram(Sinogram);
        }
    }
}
