using System;

namespace Tomograph.Library.SuperTomograph
{  
    public class DicomInformation
    {
        public string PatientName { get; set; }
        public string PatientAddress { get; set; }
        public DateTime PatientBirthDate { get; set; }        
        public string PatientSex { get; set; }
        public string PatientAge { get; set; }
        public string PatientWeight { get; set; }

        public DateTime StudyDate { get; set; }
        public string StudyDescription { get; set; }
    }
}
