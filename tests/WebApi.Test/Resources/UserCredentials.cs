namespace WebApi.Test.Resources;
public class UserCredentials
{
    private readonly string _name;
    private readonly string _email;
    private readonly string _password;

    public UserCredentials(string name, string email, string password)
    {
        _name = name;
        _email = email;
        _password = password;
    }

    public string GetName() => _name;
    public string GetEmail() => _email;
    public string GetPassword() => _password;
}