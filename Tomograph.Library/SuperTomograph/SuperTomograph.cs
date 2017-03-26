using System.Collections.Generic;
using System.Linq;
using Dicom;
using Emgu.CV;
using Emgu.CV.Structure;
using Tomograph.Library.Abstract;

namespace Tomograph.Library.SuperTomograph
{
    public class SuperTomograph
    {
        public TomographConfiguration Configuration { get; set; }
        public DicomInformation DicomInformation { get; set; }

        public Image<Gray, byte> InputImage {
            get { return _inputImage ?? (_inputImage = GetInputImage()); }
            set { _inputImage = value; }
        }

        public Image<Gray, byte> Sinogram {
            get { return _sinogram ?? (_sinogram = GetSinogram()); }
            set { _sinogram = value; }
        }
       
        public IEnumerable<Image<Gray, byte>> OutputImages
        {
            get { return _outputImages ?? (_outputImages = GetOutputImages()); }
            set { _outputImages = value; }
        }

        public DicomFile Dicom => GetDicom();
        private readonly IDicomCreator _dicomCreator;

        private Image<Gray, byte> _inputImage;
        private readonly IImageLoader _bitmapLoader;

        private Image<Gray, byte> _sinogram;
        private IEnumerable<Image<Gray, byte>> _outputImages;
        private readonly IEmitterDetectorSystem _emitterDetectorSystem;

        public SuperTomograph(IImageLoader bitmapLoader, IEmitterDetectorSystem emitterDetectorSystem,
             IDicomCreator dicomCreator)
        {
            _bitmapLoader = bitmapLoader;
            _emitterDetectorSystem = emitterDetectorSystem;
            _dicomCreator = dicomCreator;

            Configuration = new TomographConfiguration();
            DicomInformation = new DicomInformation();
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
                Sinogram, Configuration);
        }
        
        private DicomFile GetDicom()
        {
            return _dicomCreator.GetDicom(_outputImages.Last(), DicomInformation);
        }
    }
}
