using System.Drawing;

namespace Tomograph.Library.Abstract
{
    public interface ISinogramGenerator
    {
        Bitmap GetSinogram(Bitmap inputBitmap);
    }
}
