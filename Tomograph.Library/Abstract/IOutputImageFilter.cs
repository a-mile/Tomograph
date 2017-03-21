using Emgu.CV;
using Emgu.CV.Structure;

namespace Tomograph.Library.Abstract
{
    public interface IOutputImageFilter
    {
        Image<Gray,byte> GetFilteredOutputImage(Image<Gray,byte> outputImage);
    }
}
