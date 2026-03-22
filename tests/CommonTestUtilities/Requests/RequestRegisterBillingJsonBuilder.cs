using BarberBoss.Communication.Enums;
using BarberBoss.Communication.Requests;
using Bogus;

namespace CommonTestUtilities.Requests;
public class RequestRegisterBillingJsonBuilder
{
    public static RequestBillingJson Build()
    {
        List<string> _barbers = ["John's Barber Shop", "Heritage Barbershop", "Royal Cut & Shave", "Old Town Barbers"];
        List<string> _services = ["beard", "hair", "hair and beard", "hair and eyebrow", "Kids' haircut"];

        return new Faker<RequestBillingJson>()
            .RuleFor(r => r.Date, faker => DateOnly.FromDateTime(faker.Date.Past()))
            .RuleFor(r => r.BarberName, faker => faker.PickRandom(_barbers))
            .RuleFor(r => r.ClientName, faker => faker.Person.FullName)
            .RuleFor(r => r.ServiceName, faker => faker.PickRandom(_services))
            .RuleFor(r => r.Status, f => f.PickRandom<PaymentStatus>())
            .RuleFor(r => r.Amount, (f, r) => r.Status == PaymentStatus.Canceled
                ? 0
                : f.Random.Decimal(1, 1000))
            .RuleFor(r => r.PaymentMethod, faker => faker.PickRandom<PaymentMethod>())
            .RuleFor(r => r.Notes, faker => faker.Random.Bool(0.3f) ? faker.Lorem.Sentence(3) : null);
    }
}
