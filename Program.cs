using MedicalSpace.PdfApi.Models;
using MedicalSpace.PdfApi.Services;
using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;

FontManager.RegisterFont(File.OpenRead("Fonts/CairoOblique-8OyDD.otf"));
FontManager.RegisterFont(File.OpenRead("Fonts/CairoRegular-YqG9j.otf"));
FontManager.RegisterFont(File.OpenRead("Fonts/Cairo-Bold.ttf"));
FontManager.RegisterFont(File.OpenRead("Fonts/Cairo-Regular.ttf"));


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<PrescriptionPdfGenerator>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/api/prescription", (PrescriptionPdfModel model, PrescriptionPdfGenerator generator) =>
{
	try
	{
        var pdf = generator.GeneratePdf(model);

        return Results.File(pdf, "application/pdf", "Prescription.pdf");
    }
	catch (Exception ex)
	{
        return Results.Text(ex.ToString(), statusCode: 500);
    }
   
});

app.Run();
