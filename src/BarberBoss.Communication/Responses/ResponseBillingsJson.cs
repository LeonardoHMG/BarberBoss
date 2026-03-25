namespace BarberBoss.Communication.Responses;
public class ResponseBillingsJson
{
    public List<ResponseShortBillingJson> Billings { get; set; } = [];

    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public int PageSize { get; set; }
}
