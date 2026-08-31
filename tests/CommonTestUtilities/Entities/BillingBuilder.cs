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
            .CustomInstantiator(faker =>
            {
                var selectedService = string.IsNullOrWhiteSpace(serviceName)
                    ? faker.PickRandom(services)
                    : serviceName;


                var billing = Billing.Register(
                    userId: user.Id,
                    serviceDate: faker.Date.Recent(30),
                    clientName: faker.Person.FullName,
                    serviceName: selectedService,
                    amount: faker.Finance.Amount(10, 500),
                    paymentMethod: faker.PickRandom<PaymentMethod>(),
                    status: status,
                    notes: faker.Random.Bool(0.3f) ? faker.Lorem.Sentence(3) : string.Empty
                );

                billing.AttachUser(user);

                return billing;
            });
    }
}
