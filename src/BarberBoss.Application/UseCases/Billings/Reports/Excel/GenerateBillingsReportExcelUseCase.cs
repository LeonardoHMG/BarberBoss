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
    }
}
