namespace MedicalSpace.PdfApi.Models
{
    public class PrescriptionPdfModel
    {
        // Doctor
        public string DoctorName { get; set; } = "";
        public string Specialty { get; set; } = "";
        public string ClinicName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Address { get; set; } = "";
        public string ClinicLogoPath { get; set; } = "";
        public string ClinicEmail { get; set; } = "";

        public string ClinicWebsite { get; set; } = "";

        public string DoctorDegree { get; set; } = "";   // Consultant, Professor...

        public string SignaturePath { get; set; } = "";
        // Patient
        public string PatientName { get; set; } = "";
        public int Age { get; set; } = 0;
        public string Gender { get; set; } = "";
        public string Date { get; set; }

        // Medical
        public string Diagnosis { get; set; } = "";
        public string Advice { get; set; } = "";

        public List<PrescriptionDrugPdfModel> Drugs { get; set; } = new();
    }
    public class PrescriptionDrugPdfModel
    {
        public string DrugName { get; set; } = "";
        public string Dose { get; set; } = "";
        public string Frequency { get; set; } = "";
        public string Duration { get; set; } = "";
        public string Notes { get; set; } = "";
    }
}
