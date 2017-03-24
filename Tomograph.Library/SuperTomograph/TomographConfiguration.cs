using System;

namespace Tomograph.Library.SuperTomograph
{
    public class TomographConfiguration
    {
        public string InputImagePath { get; set; }
        public float Alpha { get; set; } = (float) Math.PI/180;
        public float Phi { get; set; } = (float) Math.PI/2;
        public int DetectorsCount { get; set; } = 360;
        public bool Filter { get; set; } = false;
        public int OutputImagesCount { get; set; } = 1;
    }
}
