using BarberBoss.Domain.Enums;

namespace BarberBoss.Domain.Repositories.Billings;
public record BillingFilter(
    string? BarberName,
    string? ServiceName,
    DateOnly? StartDate,
    DateOnly? EndDate,
    PaymentStatus? Status,
    PaymentMethod? PaymentMethod,
    int PageNumber,
    int PageSize,
    string OrderBy,
    bool IsDescending
);
