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

        if (billings.Count == 0)
        {
            return [];
        }

        using var workbook = new XLWorkbook();

        workbook.Author = "Leonardo Gussi";
        workbook.Style.Font.FontSize = 12;
        workbook.Style.Font.FontName = "Times New Roman";

        var worksheet = workbook.Worksheets.Add($"{startDate.ToString("dd.MM")} - {endDate.ToString("dd.MM")}");

        InsertHeader(worksheet);

        var raw = 2;

        foreach (var billing in billings)
        {
            worksheet.Cell($"A{raw}").Value = billing.ServiceName;
            worksheet.Cell($"B{raw}").Value = billing.Date.ToDateTime(TimeOnly.MinValue);
            worksheet.Cell($"B{raw}").Style.NumberFormat.Format = "dd/MM/yyyy";
            worksheet.Cell($"B{raw}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell($"C{raw}").Value = billing.PaymentMethod.ConvertPaymentMethod();
            worksheet.Cell($"C{raw}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell($"D{raw}").Value = billing.Amount;
            worksheet.Cell($"D{raw}").Style.NumberFormat.Format = $"{CURRENCY_SYMBOL} #,##0.00";
            worksheet.Cell($"D{raw}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell($"E{raw}").Value = billing.Notes;
            raw++;
        }

        worksheet.Cell($"C{raw}").Value = "Total pago:";
        worksheet.Cell($"C{raw}").Style.Font.Bold = true;
        worksheet.Cell($"C{raw}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
        worksheet.Cell($"C{raw}").Style.Fill.BackgroundColor = XLColor.FromHtml("#D9E8E8");

        worksheet.Cell($"D{raw}").Value = billings.Sum(b => b.Amount);
        worksheet.Cell($"D{raw}").Style.NumberFormat.Format = $"{CURRENCY_SYMBOL} #,##0.00";
        worksheet.Cell($"D{raw}").Style.Font.Bold = true;
        worksheet.Cell($"D{raw}").Style.Fill.BackgroundColor = XLColor.FromHtml("#D9E8E8");

        worksheet.Column("A").Width = 40; 
        worksheet.Column("B").Width = 15; 
        worksheet.Column("C").Width = 20; 
        worksheet.Column("D").Width = 15;
        worksheet.Column("E").Width = 55; 

        var file = new MemoryStream();
        workbook.SaveAs(file);

        return file.ToArray();
    }

    private void InsertHeader(IXLWorksheet worksheet)
    {
        worksheet.Cell("A1").Value = "Título";
        worksheet.Cell("B1").Value = "Data";
        worksheet.Cell("C1").Value = "Tipo de Pagamento";
        worksheet.Cell("D1").Value = "Valor";
        worksheet.Cell("E1").Value = "Descrição";

        worksheet.Cells("A1:E1").Style.Font.Bold = true;

        worksheet.Cells("A1:E1").Style.Font.FontColor = XLColor.White;

        worksheet.Cells("A1:E1").Style.Fill.BackgroundColor = XLColor.FromHtml("#205858");

        worksheet.Cells("A1:E1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
    }
}
