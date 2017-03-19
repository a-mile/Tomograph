using System.Drawing;
using Tomograph.Library.Abstract;

namespace Tomograph.Library.SuperTomograph
{
    public class SimpleBitmapLoader : IBitmapLoader
    {
        public Bitmap LoadBitmap(string bitmapPath)
        {
            return (Bitmap)Image.FromFile(bitmapPath);
        }      
    }
}
