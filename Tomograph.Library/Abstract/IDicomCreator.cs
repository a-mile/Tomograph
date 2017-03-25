using Dicom;
using Emgu.CV;
using Emgu.CV.Structure;
using Tomograph.Library.SuperTomograph;

namespace Tomograph.Library.Abstract
{
    public interface IDicomCreator
    {
        DicomFile GetDicom(Image<Gray, byte> outputImage, DicomInformation dicomInformation);
    }
}
