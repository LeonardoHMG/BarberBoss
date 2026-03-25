using AutoMapper;
using BarberBoss.Communication.Requests;
using BarberBoss.Communication.Responses;
using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Repositories.Billings;

namespace BarberBoss.Application.AutoMapper;
public class AutoMapping: Profile
{
    public AutoMapping()
    {
        RequestToEntity();
        EntityToResponse();
        RequestToFilter();
    }

    private void RequestToEntity()
    {
        CreateMap<RequestBillingJson, Billing>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
    }

    private void EntityToResponse()
    {
        CreateMap<Billing, ResponseRegisterBillingJson>();
        CreateMap<Billing, ResponseShortBillingJson>();
    }

    private void RequestToFilter()
    {
        CreateMap<RequestGetBillingsJson, BillingFilter>()
            .ConstructUsing(src => new BillingFilter(
                src.BarberName,
                src.ServiceName,
                src.StartDate,
                src.EndDate,
                src.PageNumber,
                src.PageSize,
                src.OrderBy,
                src.IsDescending
            ));
    }
}