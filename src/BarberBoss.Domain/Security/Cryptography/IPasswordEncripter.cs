namespace BarberBoss.Domain.Security.Cryptography;
public interface IPasswordEncripter
{
    string Encrypt(string password);
    bool Verfiy(string password, string passwordHash);
}
