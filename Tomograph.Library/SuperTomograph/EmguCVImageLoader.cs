using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Tomograph.Library.Abstract;

namespace Tomograph.Library.SuperTomograph
{
    public class EmguCVImageLoader : IImageLoader
    {
        public Image<Gray, byte> LoadImage(string bitmapPath)
        {
            var mat = CvInvoke.Imread(bitmapPath, LoadImageType.Grayscale);
            return mat.ToImage<Gray, byte>();
        }
    }
}
