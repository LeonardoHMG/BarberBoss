using BarberBoss.Communication.Enums;

namespace BarberBoss.Communication.Requests;
public class RequestGetBillingsJson
{
    public string? BarberName { get; set; } 
    public string? ServiceName { get; set; }
    public string? ClientName { get; set; } 
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public decimal? MinAmount { get; set; } 
    public decimal? MaxAmount { get; set; }
    public PaymentStatus? Status { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public string OrderBy { get; set; } = "ServiceDate";
    public bool IsDescending { get; set; } = true;
}