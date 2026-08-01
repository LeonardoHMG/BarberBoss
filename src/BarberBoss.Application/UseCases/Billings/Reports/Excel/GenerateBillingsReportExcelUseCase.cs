using BarberBoss.Application.Utilities;
using BarberBoss.Domain.Repositories.Billings;
using BarberBoss.Domain.Enums;
using ClosedXML.Excel;
using BarberBoss.Domain.Extensions;

namespace BarberBoss.Application.UseCases.Billings.Reports.Excel;

public class GenerateBillingsReportExcelUseCase : IGenerateBillingsReportExcelUseCase
{
    private const string CURRENCY_SYMBOL = "R$";
    private readonly IBillingsReadOnlyRepository _repository;

    public GenerateBillingsReportExcelUseCase(IBillingsReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public async Task<byte[]> Execute(DateOnly date)
    {
        var (startDate, endDate) = DateHelper.GetWeek(date);

        var billings = await _repository.FilterByWeek(startDate, endDate);

        if (billings.Count == 0) return [];

        using var workbook = new XLWorkbook();
        workbook.Author = "Leonardo Gussi";
        workbook.Style.Font.FontSize = 12;
        workbook.Style.Font.FontName = "Times New Roman";

        var worksheet = workbook.Worksheets.Add($"{startDate:dd.MM} - {endDate:dd.MM}");

        InsertHeader(worksheet, startDate, endDate);

        var row = 5;

        foreach (var billing in billings)
        {
            worksheet.Cell($"A{row}").Value = billing.ServiceDate;
            worksheet.Cell($"B{row}").Value = "";
            worksheet.Cell($"C{row}").Value = billing.ServiceName;
            worksheet.Cell($"D{row}").Value = billing.ClientName;
            worksheet.Cell($"E{row}").Value = billing.PaymentMethod.ConvertPaymentMethod();
            worksheet.Cell($"F{row}").Value = billing.Amount;
            worksheet.Cell($"G{row}").Value = billing.Notes;
            row++;
        }

        worksheet.Column("A").Style.NumberFormat.Format = "dd/MM/yyyy HH:mm";
        worksheet.Columns("A:E").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

        worksheet.Column("F").Style.NumberFormat.Format = $"{CURRENCY_SYMBOL} #,##0.00";
        worksheet.Column("F").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

        var totalRow = row + 1;
        worksheet.Cell($"E{totalRow}").Value = "Total faturado:";
        worksheet.Cell($"E{totalRow}").Style.Font.Bold = true;
        worksheet.Cell($"E{totalRow}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

        var totalPaid = billings.Where(b => b.Status == PaymentStatus.Paid).Sum(b => b.Amount);
        worksheet.Cell($"F{totalRow}").Value = totalPaid;
        worksheet.Cell($"F{totalRow}").Style.NumberFormat.Format = $"{CURRENCY_SYMBOL} #,##0.00";
        worksheet.Cell($"F{totalRow}").Style.Font.Bold = true;
        worksheet.Cell($"F{totalRow}").Style.Fill.BackgroundColor = XLColor.FromHtml("#D9E8E8");

        worksheet.Column("A").Width = 20;
        worksheet.Column("B").Width = 20;
        worksheet.Column("C").Width = 25;
        worksheet.Column("D").Width = 20; 
        worksheet.Column("E").Width = 20; 
        worksheet.Column("F").Width = 15;
        worksheet.Column("G").Width = 45;

        if (row > 5)
        {
            var dataRange = worksheet.Range($"A5:G{row - 1}");
            dataRange.Style.Alignment.WrapText = true;
            dataRange.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Top); 
            
            for (int i = 5; i < row; i++)
            {
                var currentRow = worksheet.Row(i);
                currentRow.AdjustToContents();

                if (worksheet.Cell(i, 7).Value.ToString().Length > 40)
                {
                    if (currentRow.Height < 35) currentRow.Height = 35;
                }
            }
        }

        using var file = new MemoryStream();
        workbook.SaveAs(file);

        return file.ToArray();
    }

    private void InsertHeader(IXLWorksheet worksheet, DateTime startDate, DateTime endDate)
    {
        var titleCell = worksheet.Cell("A1");
        titleCell.Value = "Relatório de Faturamento Semanal - Barbearia do João";
        worksheet.Range("A1:G1").Merge();
        titleCell.Style.Font.Bold = true;
        titleCell.Style.Font.FontSize = 16;
        titleCell.Style.Font.FontColor = XLColor.FromHtml("#205858");
        titleCell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

        var periodCell = worksheet.Cell("A2");
        periodCell.Value = $"Período: {startDate:dd/MM/yyyy} até {endDate:dd/MM/yyyy}";
        worksheet.Range("A2:G2").Merge();
        periodCell.Style.Font.Italic = true;
        periodCell.Style.Font.FontSize = 12;
        periodCell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

        var headerRow = 4;
        worksheet.Cell($"A{headerRow}").Value = "Data";
        worksheet.Cell($"B{headerRow}").Value = "Barbeiro";
        worksheet.Cell($"C{headerRow}").Value = "Serviço";
        worksheet.Cell($"D{headerRow}").Value = "Cliente";
        worksheet.Cell($"E{headerRow}").Value = "Método de Pagamento";
        worksheet.Cell($"F{headerRow}").Value = "Valor";
        worksheet.Cell($"G{headerRow}").Value = "Observações";

        var headerRange = worksheet.Range($"A{headerRow}:G{headerRow}");
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Font.FontColor = XLColor.White;
        headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#205858");
        headerRange.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
    }
}