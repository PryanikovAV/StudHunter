using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using StudHunter.API.ModelsDto;

namespace StudHunter.API.Services.Pdf;

public interface IPdfService
{
    byte[] GenerateResumePdf(ResumeSearchDto resume);
}

public class QuestPdfService : IPdfService
{
    public QuestPdfService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerateResumePdf(ResumeSearchDto resume)
    {
        var document = new ResumePdfDocument(resume);
        return document.GeneratePdf();
    }
}