using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Enums;
using Bogus;

namespace CommonTestUtilities.Entities;
public class BillingBuilder
{
    public static List<Billing> Collection(User user, uint count = 2)
    {
        if (count == 0)
            count = 1;

        return Enumerable.Range(0, (int)count)
            .Select(_ => Build(user))
            .ToList();
    }

    public static Billing Build(User user, string? serviceName = null, PaymentStatus status = PaymentStatus.Paid)
    {
        List<string> services = ["beard", "hair", "hair and beard", "hair and eyebrow", "Kids' haircut"];

        return new Faker<Billing>()
            .RuleFor(b => b.Id, _ => Guid.NewGuid())
            .RuleFor(b => b.ServiceDate, faker => faker.Date.Recent(30))
            .RuleFor(b => b.UserId, _ => user.Id)
            .RuleFor(b => b.User, _ => user)
            .RuleFor(b => b.ClientName, faker => faker.Person.FullName)
            .RuleFor(b => b.ServiceName, faker => string.IsNullOrWhiteSpace(serviceName)
                ? faker.PickRandom(services)
                : serviceName)
            .RuleFor(b => b.Amount, faker => faker.Finance.Amount(10, 500))
            .RuleFor(b => b.PaymentMethod, faker => faker.PickRandom<PaymentMethod>())
            .RuleFor(b => b.Status, _ => status)
            .RuleFor(b => b.Notes, faker => faker.Random.Bool(0.3f) ? faker.Lorem.Sentence(3) : "")
            .RuleFor(b => b.CreatedAt, faker => faker.Date.PastOffset().UtcDateTime)
            .RuleFor(b => b.UpdatedAt, (_, billing) => billing.CreatedAt);
    }
}
