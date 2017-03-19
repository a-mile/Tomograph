using System.Drawing;

namespace Tomograph.Library.Abstract
{
    public interface IBitmapLoader
    {
        Bitmap LoadBitmap(string bitmapPath);
    }
}
