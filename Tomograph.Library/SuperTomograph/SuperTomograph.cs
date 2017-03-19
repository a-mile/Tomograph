using System.Drawing;
using Tomograph.Library.Abstract;

namespace Tomograph.Library.SuperTomograph
{
    public class SuperTomograph
    {
        public TomographConfiguration Configuration { get; set; }

        public Bitmap InputBitmap => _inputBitmap ?? (_inputBitmap = GetInputBitmap());
        public Bitmap Sinogram => _sinogram ?? (_sinogram = GetSinogram());
        public Bitmap[] OutputBitmaps => _outputBitmaps ?? (_outputBitmaps = GetOutputBitmaps());
        
        private Bitmap _inputBitmap;
        private readonly IBitmapLoader _bitmapLoader;

        private Bitmap _sinogram;
        private readonly IEmitterDetectorSystem _emitterDetectorSystem;

        private Bitmap[] _outputBitmaps;

        private readonly IOutputBitmapFilter _outputBitmapFilter;

        public SuperTomograph(IBitmapLoader bitmapLoader, IEmitterDetectorSystem emitterDetectorSystem,
            IOutputBitmapFilter outputBitmapFilter)
        {
            _bitmapLoader = bitmapLoader;
            _emitterDetectorSystem = emitterDetectorSystem;
            _outputBitmapFilter = outputBitmapFilter;
            Configuration = new TomographConfiguration();
        }

        public void CleanTomograph()
        {
            _inputBitmap = null;
            _sinogram = null;
            _outputBitmaps = null;
        }

        private Bitmap GetInputBitmap()
        {
            return _bitmapLoader.LoadBitmap(Configuration.InputBitmapPath);
        }

        private Bitmap GetSinogram()
        {
            return _emitterDetectorSystem.GetSinogram(InputBitmap,Configuration);
        }

        private Bitmap[] GetOutputBitmaps()
        {
            return _emitterDetectorSystem.GetOutputBitmapsFromSinogram(Sinogram,Configuration);
        }

        private Bitmap GetFilteredOutputBitmap(Bitmap bitmap)
        {
            return _outputBitmapFilter.GetFilteredOutputBitmap(bitmap);
        }
    }
}
