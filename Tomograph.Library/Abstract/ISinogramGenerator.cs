using System.Drawing;
using Tomograph.Library.SuperTomograph;

namespace Tomograph.Library.Abstract
{
    public interface ISinogramGenerator
    {
        Bitmap GetSinogram(Bitmap inputBitmap, TomographConfiguration configuration);
    }
}
