using System.Drawing;
using Tomograph.Library.SuperTomograph;

namespace Tomograph.Library.Abstract
{
    public interface IEmitterDetectorSystem
    {
        Bitmap GetSinogram(Bitmap inputBitmap, TomographConfiguration configuration);
        Bitmap[] GetOutputBitmapsFromSinogram(Bitmap sinogram, TomographConfiguration configuration);
    }
}
