using BarberBoss.Application.Utilities;
using BarberBoss.Domain.Repositories.Billings;
using BarberBoss.Domain.Enums;
using ClosedXML.Excel;

namespace BarberBoss.Application.UseCases.Billings.Reports.Excel;

public class GenerateBillingsReportExcelUseCase : IGenerateBillingsReportExcelUseCase
{
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

        var workbook = new XLWorkbook();

        workbook.Author = "Leonardo Gussi";
        workbook.Style.Font.FontSize = 12;
        workbook.Style.Font.FontName = "Times New Roman";

        var worksheet = workbook.Worksheets.Add($"{startDate.ToString("dd.MM")} - {endDate.ToString("dd.MM")}");

        InsertHeader(worksheet);

        var raw = 2;

        foreach (var billing in billings)
        {
            worksheet.Cell($"A{raw}").Value = billing.ServiceName;
            worksheet.Cell($"B{raw}").Value = billing.BarberName;
            worksheet.Cell($"C{raw}").Value = billing.ClientName;
            worksheet.Cell($"D{raw}").Value = billing.Date.ToDateTime(TimeOnly.MinValue);
            worksheet.Cell($"D{raw}").Style.NumberFormat.Format = "dd/MM/yyyy";
            worksheet.Cell($"E{raw}").Value = ConvertPaymentMethod(billing.PaymentMethod);
            worksheet.Cell($"F{raw}").Value = billing.Amount;
            worksheet.Cell($"G{raw}").Value = billing.Notes;
            raw++;
        }

        var file = new MemoryStream();
        workbook.SaveAs(file);

        return file.ToArray();
    }

    private string ConvertPaymentMethod(PaymentMethod payment)
    {
        return payment switch
        {
            PaymentMethod.CreditCard => "Cartão de Crédito",
            PaymentMethod.DebitCard => "Cartão de Débito",
            PaymentMethod.Cash => "Dinheiro",
            PaymentMethod.Pix => "Pix",
            PaymentMethod.Other => "Outro",
            _ => string.Empty
        };
    }

    private void InsertHeader(IXLWorksheet worksheet)
    {
        worksheet.Cell("A1").Value = "Título";
        worksheet.Cell("B1").Value = "Barbeiro";
        worksheet.Cell("C1").Value = "Cliente";
        worksheet.Cell("D1").Value = "Data";
        worksheet.Cell("E1").Value = "Tipo de Pagamento";
        worksheet.Cell("F1").Value = "Valor";
        worksheet.Cell("G1").Value = "Descrição";

        worksheet.Cells("A1:G1").Style.Font.Bold = true;

        worksheet.Cells("A1:G1").Style.Font.FontColor = XLColor.White;

        worksheet.Cells("A1:G1").Style.Fill.BackgroundColor = XLColor.FromHtml("#205858");

        worksheet.Cells("A1:G1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
    }
}
