using BarberBoss.Application.UseCases.Billings.Reports.Excel;
using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Enums;
using ClosedXML.Excel;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using Shouldly;

namespace UseCases.Test.Billings.Reports.Excel;
public class GenerateBillingsReportExcelUseCaseTest
{
    [Fact]
    public async Task Success_Returns_Empty_When_No_Billings()
    {
        var useCase = CreateUseCase(new List<Billing>());

        var result = await useCase.Execute(DateOnly.FromDateTime(DateTime.Today));

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task Success_Generates_File_When_Billings_Exist()
    {
        var user = UserBuilder.Build();
        var billings = BillingBuilder.Collection(user, count: 3);

        var useCase = CreateUseCase(billings);

        var result = await useCase.Execute(DateOnly.FromDateTime(DateTime.Today));

        result.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task Success_Excel_Contains_Correct_Total()
    {
        var user = UserBuilder.Build();
        var billings = BillingBuilder.Collection(user, count: 3);

        var useCase = CreateUseCase(billings);

        var result = await useCase.Execute(DateOnly.FromDateTime(DateTime.Today));

        using var stream = new MemoryStream(result);
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheets.First();

        var expectedTotal = billings
            .Where(b => b.Status == PaymentStatus.Paid)
            .Sum(b => b.Amount);

        var totalRow = worksheet.LastRowUsed()!.RowNumber();

        worksheet.Cell($"F{totalRow}").GetValue<decimal>().ShouldBe(expectedTotal);
    }

    private GenerateBillingsReportExcelUseCase CreateUseCase(List<Billing> billings)
    {
        var repository = new BillingsReadOnlyRepositoryBuilder().FilterByWeek(billings).Build();

        return new GenerateBillingsReportExcelUseCase(repository);
    }
}
