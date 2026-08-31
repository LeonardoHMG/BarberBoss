using BarberBoss.Domain.Enums;

namespace BarberBoss.Domain.Entities;
public class Billing
{
    public Guid Id { get; private set; }
    public DateTime ServiceDate { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;
    public string ClientName { get; private set; } = string.Empty;
    public string ServiceName { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    protected Billing() { }

    public static Billing Register(
        Guid userId,
        DateTime serviceDate,
        string clientName,
        string serviceName,
        decimal amount,
        PaymentMethod paymentMethod,
        PaymentStatus status,
        string? notes)
    {
        var now = DateTime.UtcNow;

        var billing = new Billing
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CreatedAt = now,
            UpdatedAt = now
        };

        billing.SetDetails(serviceDate, clientName, serviceName, amount, paymentMethod, status, notes);

        return billing;
    }

    public void UpdateDetails(
        DateTime serviceDate,
        string clientName,
        string serviceName,
        decimal amount,
        PaymentMethod paymentMethod,
        PaymentStatus status,
        string? notes)
    {
        SetDetails(serviceDate, clientName, serviceName, amount, paymentMethod, status, notes);
        UpdatedAt = DateTime.UtcNow;
    }

    private void SetDetails(
        DateTime serviceDate,
        string clientName,
        string serviceName,
        decimal amount,
        PaymentMethod paymentMethod,
        PaymentStatus status,
        string? notes)
    {
        ServiceDate = serviceDate;
        ClientName = clientName;
        ServiceName = serviceName;
        PaymentMethod = paymentMethod;
        Notes = notes;
        Status = status;
        Amount = status == PaymentStatus.Canceled ? 0 : amount;
    }

    internal void AttachUser(User user)
    {
        User = user;
    }
}