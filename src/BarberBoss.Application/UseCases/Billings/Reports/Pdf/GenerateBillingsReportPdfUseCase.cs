using BarberBoss.Application.UseCases.Billings.Reports.Pdf.Fonts;
using BarberBoss.Application.Utilities;
using BarberBoss.Domain.Repositories.Billings;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PdfSharp.Fonts;
using System.Reflection;

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
        var page = CreatePage(document);

        
        var table = page.AddTable();

        table.AddColumn();
        table.AddColumn("300");

        var row = table.AddRow();

        var assembly = Assembly.GetExecutingAssembly();
        var directoryName = Path.GetDirectoryName(assembly.Location);
        var pathFile = Path.Combine(directoryName!, "Logo", "BarberBoss.png");

        row.Cells[0].AddImage(pathFile);

        row.Cells[1].AddParagraph("BARBEARIA DO JOÃO");
        row.Cells[1].Format.Font = new Font { Name = FontHelper.BEBASNEUE_REGULAR, Size = 25 };
        row.Cells[1].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;

        var paragraph = page.AddParagraph();

        var title = $"Faturamento da semana {startDate} - {endDate}";

        paragraph.AddFormattedText(title, new Font { Name = FontHelper.ROBOTO_MEDIUM, Size = 15 });

        paragraph.AddLineBreak();

        var totalBillings = billings.Sum(b => b.Amount);
        paragraph.AddFormattedText($"{CURRENCY_SYMBOL} {totalBillings}", new Font { Name = FontHelper.BEBASNEUE_REGULAR, Size = 50 });

        return RenderDocument(document);
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

    private Section CreatePage(Document document)
    {
        var section = document.AddSection();

        section.PageSetup = document.DefaultPageSetup.Clone();

        section.PageSetup.PageFormat = PageFormat.A4;

        section.PageSetup.LeftMargin = 40;
        section.PageSetup.RightMargin = 40;
        section.PageSetup.TopMargin = 80;
        section.PageSetup.BottomMargin = 80;

        return section;
    }

    private byte[] RenderDocument(Document document)
    {
        var renderer = new PdfDocumentRenderer
        {
            Document = document,
        };

        renderer.RenderDocument();

        using var file = new MemoryStream();
        renderer.PdfDocument.Save(file);

        return file.ToArray();
    }
}
