using BarberBoss.Communication.Enums;
using BarberBoss.Communication.Requests;
using Bogus;

namespace CommonTestUtilities.Requests;
public class RequestRegisterBillingJsonBuilder
{
    public static RequestBillingJson Build(string? serviceName = null, PaymentStatus status = PaymentStatus.Paid)
    {
        List<string> services = ["beard", "hair", "hair and beard", "hair and eyebrow", "Kids' haircut"];

        return new Faker<RequestBillingJson>()
            .RuleFor(r => r.ServiceDate, faker =>
            {
                var date = faker.Date.Recent(30);

                return new DateTime(date.Year, date.Month, date.Day, faker.Random.Int(6, 23), faker.Random.Int(0, 59), 0);
            })
            .RuleFor(r => r.ClientName, faker => faker.Person.FullName)
            .RuleFor(r => r.ServiceName, faker => string.IsNullOrWhiteSpace(serviceName)
                ? faker.PickRandom(services)
                : serviceName)
            .RuleFor(r => r.Status, _ => status)
            .RuleFor(r => r.Amount, (f, r) => r.Status == PaymentStatus.Canceled
                ? 0
                : f.Finance.Amount(10, 500))
            .RuleFor(r => r.PaymentMethod, faker => faker.PickRandom<PaymentMethod>())
            .RuleFor(r => r.Notes, faker => faker.Random.Bool(0.3f) ? faker.Lorem.Sentence(3) : "")
            .Generate();
    }
}
