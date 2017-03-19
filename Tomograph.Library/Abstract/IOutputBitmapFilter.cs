using System.Drawing;

namespace Tomograph.Library.Abstract
{
    public interface IOutputBitmapFilter
    {
        Bitmap GetFilteredOutputBitmap(Bitmap outputBitmap);
    }
}
