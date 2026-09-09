using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Enums;
using Bogus;
using CommonTestUtilities.Cryptography;

namespace CommonTestUtilities.Entities;
public class UserBuilder
{
    public static User Build(string? role = null)
    {
        var passwordEncripter = new PasswordEncrypterBuilder().Build();

        return new Faker<User>()
            .CustomInstantiator(faker =>
            {
                var name = faker.Person.FirstName;
                var email = faker.Internet.Email(name);
                var rawPassword = faker.Internet.Password(prefix: "!1aA");
                var passwordHash = passwordEncripter.Encrypt(rawPassword);
                var userRole = role ?? Roles.BARBER;

                if (userRole == Roles.ADMIN)
                {
                    return User.RegisterAdmin(name, email, passwordHash);
                }

                return User.Register(name, email, passwordHash);
            });
    }
}
