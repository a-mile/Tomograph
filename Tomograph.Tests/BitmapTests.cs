using System.Drawing;
using NUnit.Framework;
using Tomograph.Library.Infrastructure;
using Tomograph.Library.SuperTomograph;

namespace Tomograph.Tests
{
    public class BitmapTests
    {
        [Test]
        public void LoadBitmap()
        {
            string bitmapPath = @"C:\Users\amileszko\Pictures\sample.bmp";

            SuperTomograph tomograph = IoC.Container.GetInstance<SuperTomograph>();
            tomograph.SetInputBitmapPath(bitmapPath);

            Bitmap bitmap = tomograph.InputBitmap;

            Assert.IsNotNull(bitmap);
        }
    }
}
