using ClosedXML.Excel;

namespace BarberBoss.Application.UseCases.Billings.Reports.Excel;

public class GenerateBillingsReportExcelUseCase : IGenerateBillingsReportExcelUseCase
{
    public Task<byte[]> Execute(DateOnly date)
    {
        var workbook = new XLWorkbook();

        workbook.Author = "Leonardo Gussi";
        workbook.Style.Font.FontSize = 12;
        workbook.Style.Font.FontName = "Times New Roman";

        var worksheet = workbook.Worksheets.Add("Page 1");

        InsertHeader(worksheet);
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

        worksheet.Cells("A1:G1").Style.Fill.BackgroundColor = XLColor.FromHtml("#205858");

        worksheet.Cells("A1:G1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
    }
}
