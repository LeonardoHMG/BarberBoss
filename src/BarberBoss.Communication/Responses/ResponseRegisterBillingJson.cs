namespace BarberBoss.Communication.Responses; 
public class ResponseRegisterBillingJson
{
    public Guid Id { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
}
