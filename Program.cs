using MedicalSpace.PdfApi.Models;
using MedicalSpace.PdfApi.Services;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<PrescriptionPdfGenerator>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/api/prescription",
(
    PrescriptionPdfModel model,
    PrescriptionPdfGenerator generator
) =>
{
    var pdf = generator.GeneratePdf(model);

    return Results.File(
        pdf,
        "application/pdf",
        "Prescription.pdf");
});

app.Run();