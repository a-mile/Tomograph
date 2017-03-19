﻿using System.Drawing;
using Tomograph.Library.Abstract;

namespace Tomograph.Library.SuperTomograph
{
    public class SuperTomograph
    {
        public string InputBitmapPath { get; private set; }

        public Bitmap InputBitmap => _inputBitmap ?? (_inputBitmap = GetInputBitmap());
        public Bitmap Sinogram => _sinogram ?? (_sinogram = GetSinogram());
        public Bitmap OutputBitmap => _outputBitmap ?? (_outputBitmap = GetOutputBitmap());
        public Bitmap FilteredOutputBitmap
            => _filteredOutputBitmap ?? (_filteredOutputBitmap = GetFilteredOutputBitmap());

        private Bitmap _inputBitmap;
        private readonly IBitmapLoader _bitmapLoader;

        private Bitmap _sinogram;
        private readonly ISinogramGenerator _sinogramGenerator;

        private Bitmap _outputBitmap;
        private readonly IOutputBitmapGenerator _outputBitmapGenerator;

        private Bitmap _filteredOutputBitmap;
        private readonly IOutputBitmapFilter _outputBitmapFilter;

        public SuperTomograph(IBitmapLoader bitmapLoader, ISinogramGenerator sinogramGenerator,
            IOutputBitmapGenerator outputBitmapGenerator, IOutputBitmapFilter outputBitmapFilter)
        {         
            _bitmapLoader = bitmapLoader;
            _sinogramGenerator = sinogramGenerator;
            _outputBitmapGenerator = outputBitmapGenerator;
            _outputBitmapFilter = outputBitmapFilter;
        }

        public void SetInputBitmapPath(string path)
        {
            InputBitmapPath = path;
            _inputBitmap = null;
            _sinogram = null;
            _outputBitmap = null;
        }

        private Bitmap GetInputBitmap()
        {
            return _bitmapLoader.LoadBitmap(InputBitmapPath);
        }

        private Bitmap GetSinogram()
        {
            return _sinogramGenerator.GetSinogram(InputBitmap);
        }

        private Bitmap GetOutputBitmap()
        {
            return _outputBitmapGenerator.GetOutputBitmap(Sinogram);
        }

        private Bitmap GetFilteredOutputBitmap()
        {
            return _outputBitmapFilter.GetFilteredOutputBitmap(OutputBitmap);
        }
    }
}
