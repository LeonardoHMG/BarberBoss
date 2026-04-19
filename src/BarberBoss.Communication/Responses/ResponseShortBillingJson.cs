using BarberBoss.Communication.Enums;

namespace BarberBoss.Communication.Responses;
public class ResponseShortBillingJson
{
    public Guid Id { get; set; }
    public DateTime ServiceDate { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; }
}
