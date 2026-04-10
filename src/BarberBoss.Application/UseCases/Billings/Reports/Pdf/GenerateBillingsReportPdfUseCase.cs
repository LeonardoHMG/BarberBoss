using BarberBoss.Application.UseCases.Billings.Reports.Pdf.Colors;
using BarberBoss.Application.UseCases.Billings.Reports.Pdf.Fonts;
using BarberBoss.Application.Utilities;
using BarberBoss.Domain.Extensions;
using BarberBoss.Domain.Repositories.Billings;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Fonts;
using System.Reflection;

namespace BarberBoss.Application.UseCases.Billings.Reports.Pdf;

public class GenerateBillingsReportPdfUseCase : IGenerateBillingsReportPdfUseCase
{
    private const string CURRENCY_SYMBOL = "R$";
    private const int HEIGHT_ROW_BILLING_TABLE = 25;

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

        CreateHeaderWithPhotoAndName(page);

        var totalBillings = billings.Sum(b => b.Amount);
        CreateWeeklyBillingSection(page, startDate, endDate, totalBillings);

        foreach(var billing in billings)
        {
            var table = CreateBillingTable(page);

            var row = table.AddRow();
            row.Height = HEIGHT_ROW_BILLING_TABLE;
           
            AddBillingServiceName(row.Cells[0], billing.ServiceName);
            AddHeaderForAmount(row.Cells[3]);

            row = table.AddRow();
            row.Height = HEIGHT_ROW_BILLING_TABLE;

            row.Cells[0].AddParagraph(billing.CreatedAt.ToString("dd 'de' MMMM 'de' yyyy"));
            SetStyleBaseForBillingInformation(row.Cells[0]);
            row.Cells[0].Format.LeftIndent = 9;

            row.Cells[1].AddParagraph(billing.CreatedAt.ToString("t"));
            SetStyleBaseForBillingInformation(row.Cells[1]); 

            row.Cells[2].AddParagraph(billing.PaymentMethod.ConvertPaymentMethod());
            SetStyleBaseForBillingInformation(row.Cells[2]);

            AddAmountForBilling(row.Cells[3], billing.Amount);

            if (string.IsNullOrWhiteSpace(billing.Notes) == false)
            {
                var descriptionRow = table.AddRow();
                descriptionRow.Height = HEIGHT_ROW_BILLING_TABLE;

                descriptionRow.Cells[0].AddParagraph(billing.Notes);
                
                descriptionRow.Cells[0].Format.Font = new Font
                {
                    Name = FontHelper.ROBOTO_REGULAR,
                    Size = 10,
                    Color = ColorsHelper.DESCRIPTION
                };

                descriptionRow.Cells[0].Shading.Color = ColorsHelper.GRAY_LIGHT;
                descriptionRow.Cells[0].VerticalAlignment = VerticalAlignment.Center;
                descriptionRow[0].MergeRight = 2;
                descriptionRow.Cells[0].Format.LeftIndent = 9;
            }

            AddWhiteSpace(table);
        }

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


    private void CreateHeaderWithPhotoAndName(Section page)
    {
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
        row.Cells[1].Format.LeftIndent = "12";
    }

    private void CreateWeeklyBillingSection(Section page, DateOnly startDate, DateOnly endDate, decimal totalBillings)
    {
        var paragraph = page.AddParagraph();

        paragraph.Format.SpaceBefore = "40";
        paragraph.Format.SpaceAfter = "40";

        var title = $"Faturamento da semana {startDate} - {endDate}";

        paragraph.AddFormattedText(title, new Font { Name = FontHelper.ROBOTO_MEDIUM, Size = 15 });

        paragraph.AddLineBreak();

        paragraph.AddFormattedText($"{CURRENCY_SYMBOL} {totalBillings}", new Font { Name = FontHelper.BEBASNEUE_REGULAR, Size = 50 });
    }

    private Table CreateBillingTable(Section page)
    {
        var table = page.AddTable();

        table.AddColumn("143").Format.Alignment = ParagraphAlignment.Left;
        table.AddColumn("140").Format.Alignment = ParagraphAlignment.Center;
        table.AddColumn("147").Format.Alignment = ParagraphAlignment.Center;
        table.AddColumn("70").Format.Alignment = ParagraphAlignment.Right;
        return table;
    }

    private void AddBillingServiceName(Cell cell, string serviceName)
    {
        cell.AddParagraph(serviceName);
        
        cell.Format.Font = new Font 
        { 
            Name = FontHelper.BEBASNEUE_REGULAR, 
            Size = 15, 
            Color = ColorsHelper.WHITE 
        };

        cell.Shading.Color = ColorsHelper.GREEN_DARK;
        cell.VerticalAlignment = VerticalAlignment.Center;
        cell.MergeRight = 2;
        cell.Format.LeftIndent = 5;
    }

    private void AddHeaderForAmount(Cell cell)
    {
        cell.AddParagraph("VALOR");

        cell.Format.Font = new Font 
        { 
            Name = FontHelper.BEBASNEUE_REGULAR, 
            Size = 15, 
            Color = ColorsHelper.WHITE 
        };

        cell.Shading.Color = ColorsHelper.GREEN;
        cell.VerticalAlignment = VerticalAlignment.Center;
    }

    private void SetStyleBaseForBillingInformation(Cell cell)
    {
        cell.Format.Font = new Font 
        { 
            Name = FontHelper.ROBOTO_REGULAR, 
            Size = 10, 
            Color = ColorsHelper.BLACK 
        };

        cell.Shading.Color = ColorsHelper.GRAY;
        cell.VerticalAlignment = VerticalAlignment.Center;
    }

    private void AddAmountForBilling(Cell cell, decimal amount)
    {
        cell.AddParagraph($"{CURRENCY_SYMBOL} {amount}");

        cell.Format.Font = new Font 
        { 
            Name = FontHelper.ROBOTO_REGULAR, 
            Size = 10, 
            Color = ColorsHelper.BLACK 
        };

        cell.Shading.Color = ColorsHelper.WHITE;
        cell.VerticalAlignment = VerticalAlignment.Center;
    }

    private void AddWhiteSpace(Table table)
    {
        var row = table.AddRow();
        row.Height = 16;
        row.Borders.Visible = false;
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
