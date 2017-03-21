using Emgu.CV;
using Emgu.CV.Structure;

namespace Tomograph.Library.Abstract
{
    public interface IImageLoader
    {
        Image<Gray, byte> LoadImage(string bitmapPath);
    }
}
