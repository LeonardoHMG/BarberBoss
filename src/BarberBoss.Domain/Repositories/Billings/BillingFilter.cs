using BarberBoss.Domain.Enums;

namespace BarberBoss.Domain.Repositories.Billings;
public record BillingFilter(
    string? ServiceName,
    string? ClientName,
    DateOnly? StartDate,
    DateOnly? EndDate,
    decimal? MinAmount,
    decimal? MaxAmount,
    PaymentStatus? Status,
    PaymentMethod? PaymentMethod,
    string? BarberName,
    int PageNumber,
    int PageSize,
    string OrderBy,
    bool IsDescending
);
