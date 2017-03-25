using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using Dicom;
using Dicom.Imaging;
using Dicom.IO.Buffer;
using Emgu.CV;
using Emgu.CV.Structure;
using Tomograph.Library.Abstract;

namespace Tomograph.Library.SuperTomograph
{
    public class FoDicomCreator : IDicomCreator
    {
        public DicomFile GetDicom(Image<Gray, byte> outputImage, DicomInformation dicomInformation)
        {
            var bitmap = outputImage.Bitmap;
            bitmap = GetValidImage(bitmap);

            int rows, columns;
            byte[] pixels = GetPixels(bitmap, out rows, out columns);

            MemoryByteBuffer buffer = new MemoryByteBuffer(pixels);

            DicomDataset dataset = new DicomDataset();
            FillDataset(dataset, dicomInformation);

            dataset.Add(DicomTag.PhotometricInterpretation, PhotometricInterpretation.Rgb.Value);
            dataset.Add(DicomTag.Rows, (ushort)rows);
            dataset.Add(DicomTag.Columns, (ushort)columns);
            dataset.Add(DicomTag.BitsAllocated, (ushort)8);

            DicomPixelData pixelData = DicomPixelData.Create(dataset, true);

            pixelData.BitsStored = 8;
            pixelData.BitsAllocated = 8;
            pixelData.SamplesPerPixel = 3;
            pixelData.HighBit = 7;
            pixelData.PixelRepresentation = 0;
            pixelData.PlanarConfiguration = 0;
            pixelData.AddFrame(buffer);

            DicomFile dicomfile = new DicomFile(dataset);

            return dicomfile;
        }

        private void FillDataset(DicomDataset dataset, DicomInformation dicomInformation)
        {            
            dataset.Add(DicomTag.SOPClassUID, DicomUID.SecondaryCaptureImageStorage);
            dataset.Add(DicomTag.StudyInstanceUID, GenerateUid());
            dataset.Add(DicomTag.SeriesInstanceUID, GenerateUid());
            dataset.Add(DicomTag.SOPInstanceUID, GenerateUid());

            dataset.Add(DicomTag.PatientName, dicomInformation.PatientName);
            dataset.Add(DicomTag.PatientAddress, dicomInformation.PatientAddress);
            dataset.Add(DicomTag.PatientBirthDate, dicomInformation.PatientBirthDate);
            dataset.Add(DicomTag.PatientSex, dicomInformation.PatientSex);
            dataset.Add(DicomTag.PatientAge, dicomInformation.PatientAge);
            dataset.Add(DicomTag.PatientWeight, dicomInformation.PatientWeight);
            dataset.Add(DicomTag.StudyDate, dicomInformation.StudyDate);
            dataset.Add(DicomTag.StudyTime, dicomInformation.StudyDate);
            dataset.Add(DicomTag.StudyDescription, dicomInformation.StudyDescription);
        }

        private DicomUID GenerateUid()
        {
            StringBuilder uid = new StringBuilder();
            uid.Append("1.08.1982.10121984.2.0.07").Append('.').Append(DateTime.UtcNow.Ticks);

            return new DicomUID(uid.ToString(), "SOP Instance UID", DicomUidType.SOPInstance);
        }

        private Bitmap GetValidImage(Bitmap bitmap)
        {
            if (bitmap.PixelFormat != PixelFormat.Format24bppRgb)
            {
                Bitmap old = bitmap;

                using (old)
                {
                    bitmap = new Bitmap(old.Width, old.Height, PixelFormat.Format24bppRgb);

                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.DrawImage(old, 0, 0, old.Width, old.Height);
                    }
                }
            }

            return bitmap;
        }

        private byte[] GetPixels(Bitmap image, out int rows, out int columns)
        {
            rows = image.Height;
            columns = image.Width;

            if (rows % 2 != 0 && columns % 2 != 0)
                --columns;

            BitmapData data = image.LockBits(new Rectangle(0, 0, columns, rows), ImageLockMode.ReadOnly, image.PixelFormat);

            IntPtr bmpData = data.Scan0;

            try
            {
                int stride = columns * 3;
                int size = rows * stride;

                byte[] pixelData = new byte[size];

                for (int i = 0; i < rows; ++i)
                    Marshal.Copy(new IntPtr(bmpData.ToInt64() + i * data.Stride), pixelData, i * stride, stride);

                SwapRedBlue(pixelData);

                return pixelData;
            }
            finally
            {
                image.UnlockBits(data);
            }
        }

        private void SwapRedBlue(byte[] pixels)
        {
            for (int i = 0; i < pixels.Length; i += 3)
            {
                byte temp = pixels[i];
                pixels[i] = pixels[i + 2];
                pixels[i + 2] = temp;
            }
        }
    }
}
