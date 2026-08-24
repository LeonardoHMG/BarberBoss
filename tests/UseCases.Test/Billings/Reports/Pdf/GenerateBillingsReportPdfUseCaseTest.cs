using BarberBoss.Application.UseCases.Billings.Reports.Pdf;
using BarberBoss.Domain.Entities;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using Shouldly;
using System.Text;

namespace UseCases.Test.Billings.Reports.Pdf;
public class GenerateBillingsReportPdfUseCaseTest
{
    [Fact]
    public async Task Success_Returns_Empty_When_No_Billings()
    {
        var useCase = CreateUseCase(new List<Billing>());

        var result = await useCase.Execute(DateOnly.FromDateTime(DateTime.Today));

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task Success_Generates_Valid_Pdf_When_Billings_Exist()
    {
        var user = UserBuilder.Build();
        var billings = BillingBuilder.Collection(user, count: 3);

        var useCase = CreateUseCase(billings);

        var result = await useCase.Execute(DateOnly.FromDateTime(DateTime.Today));

        result.ShouldNotBeEmpty();

        var pdfHeader = Encoding.ASCII.GetString(result, 0, 4);
        pdfHeader.ShouldBe("%PDF");
    }

    private GenerateBillingsReportPdfUseCase CreateUseCase(List<Billing> billings)
    {
        var repository = new BillingsReadOnlyRepositoryBuilder().FilterByWeek(billings).Build();

        return new GenerateBillingsReportPdfUseCase(repository);
    }
}
