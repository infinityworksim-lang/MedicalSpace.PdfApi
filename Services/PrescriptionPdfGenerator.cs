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
                .Table(table =>
                {
                    // عمودان متساويان
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    // الصف الأول: اليسار = Patient Name, اليمين = Date
                    table.Cell().Element(cell =>
                    {
                        cell.Padding(4).Row(row =>
                        {
                            row.ConstantItem(100).Text("Patient Name:").FontSize(12).Bold();
                            row.ConstantItem(10).Text(":").FontSize(12).AlignCenter().Bold();
                            row.RelativeItem().Text(model.PatientName ?? "").FontSize(12).Bold();
                        });
                    });

                    table.Cell().Element(cell =>
                    {
                        cell.Padding(4).Row(row =>
                        {
                            row.ConstantItem(80).Text("Date:").FontSize(12).Bold();
                            row.ConstantItem(10).Text(":").FontSize(12).AlignCenter().Bold();
                            row.RelativeItem().Text(model.Date ?? "").FontSize(12).Bold();
                        });
                    });

                    // الصف الثاني: اليسار = Age, اليمين = Gender
                    table.Cell().Element(cell =>
                    {
                        cell.Padding(4).Row(row =>
                        {
                            row.ConstantItem(100).Text("Age:").FontSize(12).Bold();
                            row.ConstantItem(10).Text(":").FontSize(12).AlignCenter().Bold();
                            row.RelativeItem().Text(model.Age.ToString()).FontSize(12).Bold();
                        });
                    });

                    table.Cell().Element(cell =>
                    {
                        cell.Padding(4).Row(row =>
                        {
                            row.ConstantItem(80).Text("Gender:").FontSize(12).Bold();
                            row.ConstantItem(10).Text(":").FontSize(12).AlignCenter().Bold();
                            row.RelativeItem().Text(model.Gender ?? "").FontSize(12).Bold();
                        });
                    });
                });
        }
        private void DrawPrescription(ColumnDescriptor column, PrescriptionPdfModel model)
        {
            column.Item().PaddingTop(15);

            column.Item()
                .Text("℞ Prescription")
                .FontSize(18)
                .Bold();

            column.Item().PaddingTop(10);

            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(30);
                    columns.RelativeColumn(4);
                    columns.RelativeColumn(2.5f);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(3);
                });

                // Header
                table.Header(header =>
                {
                    void Header(string title)
                    {
                        header.Cell()
                            .Background("#2563EB")
                            .Border(1)
                            .BorderColor(Colors.White)
                            .Padding(7)
                            .AlignCenter()
                            .Text(title)
                            .FontColor(Colors.White)
                            .Bold()
                            .FontSize(13);
                    }

                    Header("#");
                    Header("Medication");
                    Header("SIG");
                    Header("Duration");
                    Header("Notes");
                });

                int i = 1;

                foreach (var drug in model.Drugs)
                {
                    var background = i % 2 == 0
                        ? Colors.Grey.Lighten4
                        : Colors.White;

                    // #
                    table.Cell()
                        .Background(background)
                        .BorderBottom(1)
                        .BorderColor(Colors.Grey.Lighten2)
                        .Padding(6)
                        .AlignCenter()
                        .Text(i.ToString())
                        .FontSize(12);

                    // Drug
                    table.Cell()
                        .Background(background)
                        .BorderBottom(1)
                        .BorderColor(Colors.Grey.Lighten2)
                        .Padding(6)
                        .Text(drug.DrugName ?? "")
                        .Bold()
                        .FontSize(13);

                    // SIG
                    table.Cell()
                        .Background(background)
                        .BorderBottom(1)
                        .BorderColor(Colors.Grey.Lighten2)
                        .Padding(6)
                        .Text($"{drug.Dose}   {drug.Frequency}")
                        .FontSize(12).Bold();

                    // Duration
                    table.Cell()
                        .Background(background)
                        .BorderBottom(1)
                        .BorderColor(Colors.Grey.Lighten2)
                        .Padding(6)
                        .AlignCenter()
                        .Text(drug.Duration ?? "")
                        .FontSize(12).Bold();

                    // Notes
                    table.Cell()
                        .Background(background)
                        .BorderBottom(1)
                        .BorderColor(Colors.Grey.Lighten2)
                        .Padding(6)
                        .Text(drug.Notes ?? "")
                        .FontSize(12).Bold();

                    i++;
                }
            });

            if (!string.IsNullOrWhiteSpace(model.Diagnosis))
            {
                column.Item().PaddingTop(18);

                column.Item()
                    .Text("Diagnosis")
                    .FontSize(15)
                    .Bold();

                column.Item()
                    .Border(1)
                    .BorderColor(Colors.Grey.Lighten2)
                    .Padding(10)
                    .Text(model.Diagnosis)
                    .FontSize(13).Bold();
            }

            if (!string.IsNullOrWhiteSpace(model.Advice))
            {
                column.Item().PaddingTop(15);

                column.Item()
                    .Text("Advice")
                    .FontSize(15)
                    .Bold();

                column.Item()
                    .Border(1)
                    .BorderColor(Colors.Grey.Lighten2)
                    .Padding(10)
                    .Text(model.Advice)
                    .FontSize(13).Bold();
            }
        }
        private void DrawFooter(PageDescriptor page, PrescriptionPdfModel model)
        {
            page.Footer()
                .AlignRight()
                .Text(text =>
                {
                    //text.Span("Dr. ").Bold();

                    text.Span(model.DoctorName).FontSize(14)
                        .Bold();
                });
        }
    }
}
