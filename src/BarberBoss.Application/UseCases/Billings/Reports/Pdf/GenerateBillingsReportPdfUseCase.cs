using BarberBoss.Application.UseCases.Billings.Reports.Pdf.Fonts;
using BarberBoss.Application.Utilities;
using BarberBoss.Domain.Repositories.Billings;
using MigraDoc.DocumentObjectModel;
using PdfSharp.Fonts;

namespace BarberBoss.Application.UseCases.Billings.Reports.Pdf;

public class GenerateBillingsReportPdfUseCase : IGenerateBillingsReportPdfUseCase
{
    private const string CURRENCY_SYMBOL = "R$";
    private readonly IBillingsReadOnlyRepository _repository;

    public GenerateBillingsReportPdfUseCase(IBillingsReadOnlyRepository repository)
    {
        _repository = repository;

        GlobalFontSettings.FontResolver = new BillingReportFontResolver();
    }

    public async Task<byte[]> Execute(DateOnly date)
    {
        var (startDate, endDate) = DateHelper.GetWeek(date);

        var billings = await _repository.FilterByWeek(startDate, endDate);

        if (billings.Count == 0)
        {
            return [];
        }

        var document = CreateDocument(startDate, endDate);

        return [];
    }

    private Document CreateDocument(DateOnly startDate, DateOnly endDate)
    {
       var document = new Document();

        document.Info.Title = $"Faturamento da semana {startDate} - {endDate}";
        document.Info.Author = "Leonardo Gussi";

        var style = document.Styles["Normal"];
        style!.Font.Name = FontHelper.BEBASNEUE_REGULAR;

        return document;
    }
}
