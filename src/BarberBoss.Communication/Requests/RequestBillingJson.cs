using BarberBoss.Communication.Enums;

namespace BarberBoss.Communication.Requests;
public class RequestBillingJson
{
    public DateTime ServiceDate { get; set; }
    public string BarberName { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus Status { get; set; }
    public string Notes { get; set; } = string.Empty;
}

