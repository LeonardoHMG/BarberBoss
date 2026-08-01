using BarberBoss.Domain.Enums;

namespace BarberBoss.Domain.Entities;
public class Billing
{
    public Guid Id { get; private set; }
    public DateTime ServiceDate { get; set; }

    public Guid UserId { get; private set; }
    public User User { get; set; } = default!;

    public string ClientName { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus Status { get; set; }
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; set; }

    protected Billing() { }

    public Billing(
        DateTime serviceDate,
        Guid userId,
        string clientName,
        string serviceName,
        decimal amount,
        PaymentMethod paymentMethod,
        PaymentStatus status,
        string? notes = null)
    {
        Id = Guid.NewGuid();
        ServiceDate = serviceDate;
        UserId = userId;
        ClientName = clientName;
        ServiceName = serviceName;
        Amount = amount;
        PaymentMethod = paymentMethod;
        Status = status;
        Notes = notes;

        var now = DateTime.Now;
        CreatedAt = now;
        UpdatedAt = now;
    }
}