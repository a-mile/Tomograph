using System.Collections.Generic;
using Emgu.CV;
using Emgu.CV.Structure;
using Tomograph.Library.SuperTomograph;

namespace Tomograph.Library.Abstract
{
    public interface IEmitterDetectorSystem
    {
        Image<Gray,byte> GetSinogram(Image<Gray, byte> inputImage, TomographConfiguration configuration);
        IEnumerable<Image<Gray, byte>> GetOutputImagesFromSinogram(Image<Gray, byte> inputImage, Image<Gray, byte> sinogram, TomographConfiguration configuration);
    }
}
