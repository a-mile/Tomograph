using System.Drawing;

namespace Tomograph.Library.Abstract
{
    public interface IOutputBitmapGenerator
    {
        Bitmap GetOutputBitmap(Bitmap sinogram);
    }
}
