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

                    page.DefaultTextStyle(x => x
                        .FontFamily("Cairo"));

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
        //private void DrawHeader(PageDescriptor page, PrescriptionPdfModel model)
        //{
        //    page.Header().Column(column =>
        //    {
        //        column.Item().Row(row =>
        //        {
        //            // Logo
        //            row.ConstantItem(80)
        //               .Height(80)
        //               .AlignMiddle()
        //               .Element(container =>
        //               {
        //                   if (!string.IsNullOrWhiteSpace(model.ClinicLogoPath) &&
        //                       File.Exists(model.ClinicLogoPath))
        //                   {
        //                       container.Image(model.ClinicLogoPath);
        //                   }
        //               });

        //            // Doctor Info
        //            row.RelativeItem()
        //               .PaddingLeft(15)
        //               .Column(info =>
        //               {
        //                   info.Spacing(2);

        //                   info.Item()
        //                       .Text(model.DoctorName)
        //                       .FontSize(22)
        //                       .Bold();

        //                   if (!string.IsNullOrWhiteSpace(model.DoctorDegree))
        //                   {
        //                       info.Item()
        //                           .Text(model.DoctorDegree)
        //                           .FontSize(13)
        //                           .FontColor(Colors.Grey.Darken2);
        //                   }

        //                   info.Item()
        //                       .Text(model.Specialty)
        //                       .FontSize(14)
        //                       .FontColor(Colors.Blue.Medium);

        //                   info.Item()
        //                       .Text(model.ClinicName)
        //                       .FontSize(14)
        //                       .Bold();

        //                   info.Item()
        //                       .Text(model.Address)
        //                       .FontSize(14);

        //                   info.Item()
        //                       .Text($"☎ {model.Phone}").FontSize(14);

        //                   if (!string.IsNullOrWhiteSpace(model.ClinicEmail))
        //                       info.Item().Text($"✉ {model.ClinicEmail}");

        //                   if (!string.IsNullOrWhiteSpace(model.ClinicWebsite))
        //                       info.Item().Text(model.ClinicWebsite);
        //               });
        //        });

        //        column.Item()
        //            .PaddingTop(12)
        //            .LineHorizontal(2)
        //            .LineColor(Colors.Blue.Medium);
        //    });
        //}
        private void DrawHeader(PageDescriptor page, PrescriptionPdfModel model)
        {
            page.Header().Column(header =>
            {
                header.Item().Row(row =>
                {
                    // =========================
                    // Doctor Information (Left)
                    // =========================
                    row.RelativeItem().Column(info =>
                    {
                        info.Spacing(3);

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

                        if (!string.IsNullOrWhiteSpace(model.Specialty))
                        {
                            info.Item()
                                .Text(model.Specialty)
                                .FontSize(14)
                                .FontColor(Colors.Blue.Medium);
                        }

                        if (!string.IsNullOrWhiteSpace(model.ClinicName))
                            info.Item().Text(model.ClinicName).Bold();

                        if (!string.IsNullOrWhiteSpace(model.Address))
                            info.Item().Text(model.Address);

                        if (!string.IsNullOrWhiteSpace(model.Phone))
                            info.Item().Text($"☎ {model.Phone}");

                        if (!string.IsNullOrWhiteSpace(model.ClinicEmail))
                            info.Item().Text($"✉ {model.ClinicEmail}");

                        if (!string.IsNullOrWhiteSpace(model.ClinicWebsite))
                            info.Item().Text(model.ClinicWebsite);
                    });

                    // =========================
                    // Logo (Right)
                    // =========================
                    row.ConstantItem(90)
                        .Height(90)
                        .AlignRight()
                        .AlignMiddle()
                        .Element(container =>
                        {
                            if (!string.IsNullOrWhiteSpace(model.ClinicLogoPath)
                                && File.Exists(model.ClinicLogoPath))
                            {
                                container.Image(model.ClinicLogoPath);
                            }
                        });
                });

                header.Item()
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
                            r.ConstantItem(95).Text("Patient Name").FontSize(14);
                            r.ConstantItem(10).AlignCenter().Text(":").FontSize(14);
                            r.RelativeItem().Text(model.PatientName).FontSize(14).Bold();
                        });

                        left.Item().Row(r =>
                        {
                            r.ConstantItem(95).Text("Age").FontSize(14);
                            r.ConstantItem(10).AlignCenter().Text(":").FontSize(14);
                            r.RelativeItem().Text(model.Age).FontSize(14).Bold();
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
                            r.ConstantItem(70).Text("Date").FontSize(14);
                            r.ConstantItem(10).AlignCenter().Text(":").FontSize(14);
                            r.RelativeItem().Text(model.Date/*.ToString("dd MMM yyyy")*/).FontSize(14).Bold();
                        });

                        right.Item().Row(r =>
                        {
                            r.ConstantItem(70).Text("Gender").FontSize(14);
                            r.ConstantItem(10).AlignCenter().Text(":").FontSize(14);
                            r.RelativeItem().Text(model.Gender).FontSize(14).Bold();
                        });
                    });
                });

            column.Item()
                .PaddingTop(10);
        }
        //private void DrawPrescription(ColumnDescriptor column, PrescriptionPdfModel model)
        //{

        //    column.Item().PaddingTop(15);

        //    column.Item()
        //        .Text("℞ Prescription")
        //        .FontSize(18)
        //        .Bold();

        //    column.Item().PaddingTop(10);

        //    foreach (var drug in model.Drugs)
        //    {
        //        column.Item().BorderBottom(1)
        //            .BorderColor(Colors.Grey.Lighten2)
        //            .PaddingVertical(8)
        //            .Column(item =>
        //            {
        //                item.Item()
        //                    .Text(drug.DrugName)
        //                    .Bold()
        //                    .FontSize(13);

        //                item.Item()
        //                    .Text($"{drug.Dose}    {drug.Frequency}").FontSize(13);

        //                if (!string.IsNullOrWhiteSpace(drug.Duration))
        //                    item.Item().Text($"Duration : {drug.Duration}").FontSize(13);

        //                if (!string.IsNullOrWhiteSpace(drug.Notes))
        //                    item.Item().Text(drug.Notes)
        //                        .FontColor(Colors.Grey.Darken1).FontSize(13);
        //            });
        //    }

        //    column.Item().PaddingTop(20);

        //    if (!string.IsNullOrWhiteSpace(model.Diagnosis))
        //    {
        //        column.Item()
        //            .Text("Diagnosis")
        //            .Bold()
        //            .FontSize(16);

        //        column.Item()
        //            .PaddingTop(5)
        //            .Text(model.Diagnosis).FontSize(13);
        //    }

        //    if (!string.IsNullOrWhiteSpace(model.Advice))
        //    {
        //        column.Item()
        //            .PaddingTop(15)
        //            .Text("Advice")
        //            .Bold()
        //            .FontSize(16);

        //        column.Item()
        //            .PaddingTop(5)
        //            .Text(model.Advice).FontSize(13);
        //    }

        //}
        private void DrawPrescription(ColumnDescriptor column, PrescriptionPdfModel model)
        {
            column.Item().PaddingTop(15);

            // Title
            column.Item()
                .Text("℞ Prescription")
                .FontSize(18)
                .Bold();

            column.Item().PaddingTop(10);

            // ===========================
            // Drugs Table
            // ===========================
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(30);     // #
                    columns.RelativeColumn(4);      // Drug
                    columns.RelativeColumn(2);      // Sig
                    columns.RelativeColumn(2);      // Duration
                    columns.RelativeColumn(3);      // Notes
                });

                // ===========================
                // Header
                // ===========================
                table.Header(header =>
                {
                    void HeaderCell(string text)
                    {
                        header.Cell()
                            .Background(Colors.Blue.Medium)
                            .Border(1)
                            .BorderColor(Colors.White)
                            .PaddingVertical(8)
                            .PaddingHorizontal(5)
                            .AlignCenter()
                            .Text(text)
                            .FontColor(Colors.White)
                            .Bold()
                            .FontSize(13);
                    }

                    HeaderCell("#");
                    HeaderCell("Medication");
                    HeaderCell("SIG");
                    HeaderCell("Duration");
                    HeaderCell("Notes");
                });

                // ===========================
                // Rows
                // ===========================

                int index = 1;

                foreach (var drug in model.Drugs)
                {
                    bool even = index % 2 == 0;

                    string background = even
                        ? Colors.Grey.Lighten4
                        : Colors.White;

                    void Cell(string value, bool center = false, bool bold = false)
                    {
                        var cell = table.Cell()
                            .Background(background)
                            .BorderBottom(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .PaddingVertical(6)
                            .PaddingHorizontal(5);

                        if (center)
                            cell.AlignCenter();

                        var text = cell.Text(value ?? "");

                        text.FontSize(12);

                        if (bold)
                            text.Bold();
                    }

                    Cell(index.ToString(), true);

                    Cell(drug.DrugName, false, true);

                    Cell($"{drug.Dose} {drug.Frequency}");

                    Cell(drug.Duration, true);

                    Cell(drug.Notes);

                    index++;
                }
            });

            // ===========================
            // Diagnosis
            // ===========================

            if (!string.IsNullOrWhiteSpace(model.Diagnosis))
            {
                column.Item().PaddingTop(20);

                column.Item()
                    .Text("Diagnosis")
                    .FontSize(15)
                    .Bold();

                column.Item()
                    .PaddingTop(5)
                    .Border(1)
                    .BorderColor(Colors.Grey.Lighten2)
                    .Padding(10)
                    .Text(model.Diagnosis)
                    .FontSize(13);
            }

            // ===========================
            // Advice
            // ===========================

            if (!string.IsNullOrWhiteSpace(model.Advice))
            {
                column.Item().PaddingTop(15);

                column.Item()
                    .Text("Advice")
                    .FontSize(15)
                    .Bold();

                column.Item()
                    .PaddingTop(5)
                    .Border(1)
                    .BorderColor(Colors.Grey.Lighten2)
                    .Padding(10)
                    .Text(model.Advice)
                    .FontSize(13);
            }
        }
        private void DrawFooter(PageDescriptor page, PrescriptionPdfModel model)
        {
            page.Footer()
                .AlignRight()
                .Text(text =>
                {
                    text.Span("Dr. ");

                    text.Span(model.DoctorName).FontSize(14)
                        .Bold();
                });
        }
    }
}
