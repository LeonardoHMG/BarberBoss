using BarberBoss.Communication.Requests;
using Bogus;

namespace CommonTestUtilities.Requests;
public class RequestGetBillingsJsonBuilder
{
    public static RequestGetBillingsJson Build()
    {
        return new Faker<RequestGetBillingsJson>()
            .RuleFor(r => r.PageNumber, _ => 1)
            .RuleFor(r => r.PageSize, _ => 10)
            .RuleFor(r => r.OrderBy, _ => "ServiceDate")
            .RuleFor(r => r.IsDescending, _ => true);
    }
}