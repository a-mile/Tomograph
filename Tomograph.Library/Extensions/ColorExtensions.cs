using System;
using System.Drawing;

namespace Tomograph.Library.Extensions
{
    public static class ColorExtensions
    {
        public static int GetGrayScale(this Color color)
        {
            return (int) Math.Round(color.GetBrightness()*255.0);
        }
    }
}
