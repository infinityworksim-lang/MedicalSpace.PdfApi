using MedicalSpace.PdfApi.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace MedicalSpace.PdfApi.Services
{
    public class PrescriptionPdfGenerator
    {
        public byte[] GeneratePdf(PrescriptionPdfModel model)
        {
            using var stream = new MemoryStream();

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20);

                    page.DefaultTextStyle(x => x.FontSize(11));

                    DrawHeader(page, model);

                    page.Content().Column(column =>
                    {
                        column.Spacing(10);

                        DrawPatientInfo(column, model);

                        DrawPrescription(column, model);
                    });

                    DrawFooter(page, model);
                });

            }).GeneratePdf(stream);

            return stream.ToArray();
        }
        private void DrawHeader(PageDescriptor page, PrescriptionPdfModel model)
        {
            page.Header().Column(column =>
            {
                column.Item().Row(row =>
                {
                    // Logo
                    row.ConstantItem(80)
                       .Height(80)
                       .AlignMiddle()
                       .Element(container =>
                       {
                           if (!string.IsNullOrWhiteSpace(model.ClinicLogoPath) &&
                               File.Exists(model.ClinicLogoPath))
                           {
                               container.Image(model.ClinicLogoPath);
                           }
                       });

                    // Doctor Info
                    row.RelativeItem()
                       .PaddingLeft(15)
                       .Column(info =>
                       {
                           info.Spacing(2);

                           info.Item()
                               .Text(model.DoctorName)
                               .FontSize(22)
                               .Bold();

                           if (!string.IsNullOrWhiteSpace(model.DoctorDegree))
                           {
                               info.Item()
                                   .Text(model.DoctorDegree)
                                   .FontSize(12)
                                   .FontColor(Colors.Grey.Darken2);
                           }

                           info.Item()
                               .Text(model.Specialty)
                               .FontSize(14)
                               .FontColor(Colors.Blue.Medium);

                           info.Item()
                               .Text(model.ClinicName)
                               .Bold();

                           info.Item()
                               .Text(model.Address);

                           info.Item()
                               .Text($"☎ {model.Phone}");

                           if (!string.IsNullOrWhiteSpace(model.ClinicEmail))
                               info.Item().Text($"✉ {model.ClinicEmail}");

                           if (!string.IsNullOrWhiteSpace(model.ClinicWebsite))
                               info.Item().Text(model.ClinicWebsite);
                       });
                });

                column.Item()
                    .PaddingTop(12)
                    .LineHorizontal(2)
                    .LineColor(Colors.Blue.Medium);
            });
        }
        private void DrawPatientInfo(ColumnDescriptor column, PrescriptionPdfModel model)
        {
            column.Item()
                .PaddingTop(15)
                .Border(1)
                .BorderColor(Colors.Grey.Lighten2)
                .Padding(12)
                .Row(row =>
                {
                    // العمود الأيسر
                    row.RelativeItem().Column(left =>
                    {
                        left.Spacing(6);

                        left.Item().Row(r =>
                        {
                            r.ConstantItem(95).Text("Patient Name");
                            r.ConstantItem(10).AlignCenter().Text(":");
                            r.RelativeItem().Text(model.PatientName);
                        });

                        left.Item().Row(r =>
                        {
                            r.ConstantItem(95).Text("Age");
                            r.ConstantItem(10).AlignCenter().Text(":");
                            r.RelativeItem().Text(model.Age);
                        });
                    });

                    // مسافة بين العمودين
                    row.ConstantItem(30);

                    // العمود الأيمن
                    row.RelativeItem().Column(right =>
                    {
                        right.Spacing(6);

                        right.Item().Row(r =>
                        {
                            r.ConstantItem(70).Text("Date");
                            r.ConstantItem(10).AlignCenter().Text(":");
                            r.RelativeItem().Text(model.Date/*.ToString("dd MMM yyyy")*/);
                        });

                        right.Item().Row(r =>
                        {
                            r.ConstantItem(70).Text("Gender");
                            r.ConstantItem(10).AlignCenter().Text(":");
                            r.RelativeItem().Text(model.Gender);
                        });
                    });
                });

            column.Item()
                .PaddingTop(10);
        }
        private void DrawPrescription(ColumnDescriptor column, PrescriptionPdfModel model)
        {

            column.Item().PaddingTop(15);

            column.Item()
                .Text("℞ Prescription")
                .FontSize(18)
                .Bold();

            column.Item().PaddingTop(10);

            foreach (var drug in model.Drugs)
            {
                column.Item().BorderBottom(1)
                    .BorderColor(Colors.Grey.Lighten2)
                    .PaddingVertical(8)
                    .Column(item =>
                    {
                        item.Item()
                            .Text(drug.DrugName)
                            .Bold()
                            .FontSize(13);

                        item.Item()
                            .Text($"{drug.Dose}    {drug.Frequency}");

                        if (!string.IsNullOrWhiteSpace(drug.Duration))
                            item.Item().Text($"Duration : {drug.Duration}");

                        if (!string.IsNullOrWhiteSpace(drug.Notes))
                            item.Item().Text(drug.Notes)
                                .FontColor(Colors.Grey.Darken1);
                    });
            }

            column.Item().PaddingTop(20);

            if (!string.IsNullOrWhiteSpace(model.Diagnosis))
            {
                column.Item()
                    .Text("Diagnosis")
                    .Bold()
                    .FontSize(16);

                column.Item()
                    .PaddingTop(5)
                    .Text(model.Diagnosis);
            }

            if (!string.IsNullOrWhiteSpace(model.Advice))
            {
                column.Item()
                    .PaddingTop(15)
                    .Text("Advice")
                    .Bold()
                    .FontSize(16);

                column.Item()
                    .PaddingTop(5)
                    .Text(model.Advice);
            }

        }
        private void DrawFooter(PageDescriptor page, PrescriptionPdfModel model)
        {
            page.Footer()
                .AlignRight()
                .Text(text =>
                {
                    text.Span("Dr. ");

                    text.Span(model.DoctorName)
                        .Bold();
                });
        }
    }
}
