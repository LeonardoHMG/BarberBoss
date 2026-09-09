using BarberBoss.Communication.Requests;
using BarberBoss.Domain.Enums;
using Bogus;

namespace CommonTestUtilities.Requests;
public class RequestUpdateUserByAdminJsonBuilder
{
    public static RequestUpdateUserByAdminJson Build()
    {
        return new Faker<RequestUpdateUserByAdminJson>()
            .RuleFor(user => user.Name, faker => faker.Person.FirstName)
            .RuleFor(user => user.Email, (faker, user) => faker.Internet.Email(user.Name))
            .RuleFor(user => user.Role, faker => faker.PickRandom(Roles.ADMIN, Roles.BARBER))
            .RuleFor(user => user.IsActive, faker => faker.Random.Bool());
    }
}
