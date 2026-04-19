using BarberBoss.Domain.Enums;

namespace BarberBoss.Domain.Repositories.Billings;
public record BillingFilter(
    string? BarberName,
    string? ServiceName,
    string? ClientName,
    DateOnly? StartDate,
    DateOnly? EndDate,
    decimal? MinAmount,
    decimal? MaxAmount,
    PaymentStatus? Status,
    PaymentMethod? PaymentMethod,
    int PageNumber,
    int PageSize,
    string OrderBy,
    bool IsDescending
);
