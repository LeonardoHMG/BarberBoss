namespace BarberBoss.Domain.Repositories.Billings;
public record BillingFilter(
    string? BarberName,
    string? ServiceName,
    DateOnly? StartDate,
    DateOnly? EndDate,
    int PageNumber,
    int PageSize,
    string OrderBy,
    bool IsDescending
);
