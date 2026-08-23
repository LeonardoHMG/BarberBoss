using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Enums;
using Bogus;
using CommonTestUtilities.Cryptography;

namespace CommonTestUtilities.Entities;
public class UserBuilder
{
    public static User Build(string role = null)
    {
        var passwordEncripter = new PasswordEncrypterBuilder().Build();

        var user = new Faker<User>()
            .RuleFor(u => u.Id, _ => Guid.NewGuid())
            .RuleFor(u => u.Name, faker => faker.Person.FirstName)
            .RuleFor(u => u.Email, (faker, user) => faker.Internet.Email(user.Name))
            .RuleFor(u => u.PasswordHash, (_, user) => passwordEncripter.Encrypt(user.Name))
            .RuleFor(u => u.Role, _ => role ?? Roles.BARBER)
            .RuleFor(u => u.CreatedAt, faker => faker.Date.PastOffset().UtcDateTime)
            .RuleFor(u => u.UpdatedAt, (_, user) => user.CreatedAt);
        
        return user;
    }
}
