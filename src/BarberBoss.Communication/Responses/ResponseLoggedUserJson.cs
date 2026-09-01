namespace BarberBoss.Communication.Responses;
public class ResponseLoggedUserJson
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}
