using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tomograph.Library.Abstract
{
    public interface IOutputBitmapFilter
    {
        Bitmap GetFilteredOutputBitmap(Bitmap outputBitmap);
    }
}
