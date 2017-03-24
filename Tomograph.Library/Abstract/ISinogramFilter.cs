using Emgu.CV;
using Emgu.CV.Structure;

namespace Tomograph.Library.Abstract
{
    public interface ISinogramFilter
    {
        Image<Gray,byte> GetFilteredSinogram(Image<Gray,byte> singoram);
    }
}
