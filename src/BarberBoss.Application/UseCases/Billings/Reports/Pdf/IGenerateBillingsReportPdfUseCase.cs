namespace BarberBoss.Application.UseCases.Billings.Reports.Pdf;
public interface IGenerateBillingsReportPdfUseCase
{
    Task<byte[]> Execute(DateOnly date);
}
