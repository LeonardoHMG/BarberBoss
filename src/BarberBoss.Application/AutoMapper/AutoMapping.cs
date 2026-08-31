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
        EntityToResponse();
        RequestToFilter();
    }

    private void EntityToResponse()
    {
        CreateMap<Billing, ResponseRegisterBillingJson>();
        
        CreateMap<Billing, ResponseShortBillingJson>()
        .ForMember(dest => dest.BarberName, opt => opt.MapFrom(src => src.User.Name));
        
        CreateMap<Billing, ResponseBillingJson>()
        .ForMember(dest => dest.BarberName, opt => opt.MapFrom(src => src.User.Name));

        CreateMap<User, ResponseUserProfileJson>();
    }

    private void RequestToFilter()
    {
        CreateMap<RequestGetBillingsJson, BillingFilter>()
            .ConstructUsing(src => new BillingFilter(
                src.ServiceName,
                src.ClientName,
                src.StartDate,
                src.EndDate,
                src.MinAmount.HasValue ? src.MinAmount.Value : 0,
                src.MaxAmount.HasValue ? src.MaxAmount.Value : 0,
                src.Status.HasValue ? (Domain.Enums.PaymentStatus)src.Status.Value : null,
                src.PaymentMethod.HasValue ? (Domain.Enums.PaymentMethod)src.PaymentMethod.Value : null,
                src.BarberName,
                src.PageNumber,
                src.PageSize,
                src.OrderBy,
                src.IsDescending
            ));
    }
}