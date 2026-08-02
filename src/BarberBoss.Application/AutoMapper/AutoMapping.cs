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
        
        CreateMap<RequestRegisterUserJson, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
    }

    private void EntityToResponse()
    {
        CreateMap<Billing, ResponseRegisterBillingJson>();
        CreateMap<Billing, ResponseShortBillingJson>();
        CreateMap<Billing, ResponseBillingJson>();
    }

    private void RequestToFilter()
    {
        CreateMap<RequestGetBillingsJson, BillingFilter>()
            .ConstructUsing(src => new BillingFilter(
                src.BarberName,
                src.ServiceName,
                src.ClientName,
                src.StartDate,
                src.EndDate,
                src.MinAmount.HasValue ? src.MinAmount.Value : 0,
                src.MaxAmount.HasValue ? src.MaxAmount.Value : 0,
                src.Status.HasValue ? (Domain.Enums.PaymentStatus)src.Status.Value : null,
                src.PaymentMethod.HasValue ? (Domain.Enums.PaymentMethod)src.PaymentMethod.Value : null,
                src.PageNumber,
                src.PageSize,
                src.OrderBy,
                src.IsDescending
            ));
    }
}