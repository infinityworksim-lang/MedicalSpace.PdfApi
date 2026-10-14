using MedicalSpace.PdfApi.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace MedicalSpace.PdfApi.Services
{
    public class PrescriptionPdfGenerator
    {
        private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(20) };

        private const int MaxLogoBytes = 5 * 1024 * 1024;

        // Generate Pdf
        public async Task<byte[]> GeneratePdfAsync(PrescriptionPdfModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            // Download the logo before QuestPDF lays out the document.
            // ClinicLogoPath must be an HTTPS URL from Supabase Storage.
            byte[]? logoBytes = await DownloadClinicLogoAsync(model.ClinicLogoPath);

            using var stream = new MemoryStream();

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(15);

                    page.DefaultTextStyle(x => x.FontFamily("Cairo"));

                    // Draw Header
                    DrawHeader(page, model, logoBytes);

                    // Draw Content (Between Header and Footer)
                    page.Content().Column(column =>
                    {
                        column.Spacing(5);
                        DrawPatientInfo(column, model);
                        DrawPrescription(column, model);
                    });

                    // Draw Footer
                    DrawFooter(page, model);
                });

            }).GeneratePdf(stream);

            return stream.ToArray();
        }

        // Download Clinic Logo
        private static async Task<byte[]?> DownloadClinicLogoAsync(string? logoUrl)
        {
            if (string.IsNullOrWhiteSpace(logoUrl)) { return null; }
               
            if (!Uri.TryCreate(logoUrl, UriKind.Absolute, out var uri) ||
                uri.Scheme != Uri.UriSchemeHttps ||
                !(uri.Host.Equals("supabase.co", StringComparison.OrdinalIgnoreCase) ||
                  uri.Host.EndsWith(".supabase.co", StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Clinic Logo Path must be a valid HTTPS URL from Supabase Storage.");
            }

            using var response = await Http.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            var mediaType = response.Content.Headers.ContentType?.MediaType;
            if (mediaType is null || !mediaType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"The clinic logo URL did not return an image. Content-Type: {mediaType ?? "(missing)"}");
            }
            if (response.Content.Headers.ContentLength is long contentLength && contentLength > MaxLogoBytes)
            {
                throw new InvalidOperationException("The clinic logo image must not exceed 5 MB.");
            }

            await using var input = await response.Content.ReadAsStreamAsync();
            using var output = new MemoryStream();
            var buffer = new byte[81920];
            int bytesRead;

            while ((bytesRead = await input.ReadAsync(buffer.AsMemory(0, buffer.Length))) > 0)
            {
                if (output.Length + bytesRead > MaxLogoBytes)
                {
                    throw new InvalidOperationException("The clinic logo image must not exceed 5 MB.");
                }

                await output.WriteAsync(buffer.AsMemory(0, bytesRead));
            }

            return output.ToArray();
        }

        // Draw Header
        private void DrawHeader(PageDescriptor page, PrescriptionPdfModel model, byte[]? logoBytes)
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
                        info.Spacing(0); // 3

                        // Doctor name
                        info.Item().Text(model.DoctorName).FontSize(18).LineHeight(0.9f).Bold();

                        // Doctor Specialty
                        if (!string.IsNullOrWhiteSpace(model.Specialty)) { info.Item().Text(model.Specialty).FontSize(13).LineHeight(0.9f).FontColor(Colors.Blue.Medium); }

                        // Doctor Degree
                        if (!string.IsNullOrWhiteSpace(model.DoctorDegree)) { info.Item().Text(model.DoctorDegree).FontSize(13).LineHeight(0.9f).FontColor(Colors.Grey.Darken2); }

                        //// Clinic Name
                        //if (!string.IsNullOrWhiteSpace(model.ClinicName)) { info.Item().Text(model.ClinicName).FontSize(13).LineHeight(0.9f).Bold(); }

                        // Clinic Address
                        if (!string.IsNullOrWhiteSpace(model.Address)) { info.Item().Text(model.Address).FontSize(13).LineHeight(0.9f); }

                        // Phone
                        if (!string.IsNullOrWhiteSpace(model.Phone)) { info.Item().Text($"{model.Phone}").FontSize(13).LineHeight(0.9f); }

                        //// Email
                        //if (!string.IsNullOrWhiteSpace(model.ClinicEmail)) { info.Item().Text($"{model.ClinicEmail}").FontSize(13); }

                        //// Website
                        //if (!string.IsNullOrWhiteSpace(model.ClinicWebsite)) { info.Item().Text(model.ClinicWebsite).FontSize(13); }       
                    });

                    // =========================
                    // Logo (Right)
                    // =========================
                    row.ConstantItem(90).Height(90).AlignRight().AlignMiddle().Element(container =>
                        {
                            if (logoBytes is { Length: > 0 })
                            {
                                container.Image(logoBytes).FitArea();
                            }
                        });
                });

                // Line
                header.Item().PaddingTop(6).LineHorizontal(2).LineColor(Colors.Blue.Medium);
            });
        }

        // Draw Patient Info
        private void DrawPatientInfo(ColumnDescriptor column, PrescriptionPdfModel model)
        {
            column.Item().PaddingTop(6).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(12)
                .Table(table =>
                {
                    // عمودان بنسبة 1:2
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2); // 2/3
                        columns.RelativeColumn(1); // 1/3
                    });

                    // الصف الأول: اليسار = Patient Name, اليمين = Date
                    table.Cell().Element(cell =>
                    {
                        cell.Padding(4).Row(row =>
                        {
                            row.ConstantItem(95).Text("Patient Name").FontSize(14).Bold();
                            row.ConstantItem(10).Text(":").FontSize(12).AlignCenter().Bold();
                            row.RelativeItem().Text(model.PatientName ?? "").FontSize(14).Bold();
                        });
                    });

                    table.Cell().Element(cell =>
                    {
                        cell.Padding(4).Row(row =>
                        {
                            row.ConstantItem(75).Text("Date").FontSize(14).Bold();
                            row.ConstantItem(10).Text(":").FontSize(12).AlignCenter().Bold();
                            row.RelativeItem().Text(model.Date ?? "").FontSize(14).Bold();
                        });
                    });

                    // الصف الثاني: اليسار = Age, اليمين = Gender
                    table.Cell().Element(cell =>
                    {
                        cell.Padding(4).Row(row =>
                        {
                            row.ConstantItem(95).Text("Age").FontSize(14).Bold();
                            row.ConstantItem(10).Text(":").FontSize(12).AlignCenter().Bold();
                            row.RelativeItem().Text(model.Age.ToString()).FontSize(14).Bold();
                        });
                    });

                    table.Cell().Element(cell =>
                    {
                        cell.Padding(4).Row(row =>
                        {
                            row.ConstantItem(75).Text("Gender").FontSize(14).Bold();
                            row.ConstantItem(10).Text(":").FontSize(12).AlignCenter().Bold();
                            row.RelativeItem().Text(model.Gender ?? "").FontSize(14).Bold();
                        });
                    });
                });
        }

        // Draw Prescription
        private void DrawPrescription(ColumnDescriptor column, PrescriptionPdfModel model)
        {
            column.Item().PaddingTop(8);

            column.Item()
                .Text("℞ Prescription")
                .FontSize(18)
                .Bold();

           // column.Item().PaddingTop(3);

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
                        .FontSize(13).Bold();

                    // Duration
                    table.Cell()
                        .Background(background)
                        .BorderBottom(1)
                        .BorderColor(Colors.Grey.Lighten2)
                        .Padding(6)
                        .AlignCenter()
                        .Text(drug.Duration ?? "")
                        .FontSize(13).Bold();

                    // Notes
                    table.Cell()
                        .Background(background)
                        .BorderBottom(1)
                        .BorderColor(Colors.Grey.Lighten2)
                        .Padding(6)
                        .Text(drug.Notes ?? "")
                        .FontSize(13).Bold();

                    i++;
                }
            });

            // Diagnosis
            if (!string.IsNullOrWhiteSpace(model.Diagnosis))
            {
                column.Item().PaddingTop(8);

                column.Item()
                    .Text("Diagnosis")
                    .FontSize(15)
                    .Bold();

                column.Item()
                    .Border(1)
                    .BorderColor(Colors.Grey.Lighten2)
                    .Padding(7)
                    .Text(model.Diagnosis)
                    .FontSize(13).Bold();
            }

            // Advice
            if (!string.IsNullOrWhiteSpace(model.Advice))
            {
                column.Item().PaddingTop(8);

                column.Item()
                    .Text("Advice")
                    .FontSize(15)
                    .Bold();

                column.Item()
                    .Border(1)
                    .BorderColor(Colors.Grey.Lighten2)
                    .Padding(7)
                    .Text(model.Advice)
                    .FontSize(13).Bold();
            }
        }

        // Draw Footer
        private void DrawFooter(PageDescriptor page, PrescriptionPdfModel model)
        {
            page.Footer().AlignRight().Text(text =>
                {
                    //text.Span("Dr. ").Bold();

                    text.Span(model.DoctorName).FontSize(14).Bold();
                });
        }
    }
}
