using AutoMapper;
using BarberBoss.Communication.Responses;
using BarberBoss.Domain.Repositories.Billings;
using BarberBoss.Domain.Services.LoggedUser;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;

namespace BarberBoss.Application.UseCases.Billings.GetById;

public class GetBillingByIdUseCase : IGetBillingByIdUseCase
{
    private readonly IBillingsReadOnlyRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILoggedUser _loggedUser;

    public GetBillingByIdUseCase(
        IBillingsReadOnlyRepository repository, 
        IMapper mapper,
        ILoggedUser loggedUser)
    {
        _repository = repository;
        _mapper = mapper;
        _loggedUser = loggedUser;
    }

    public async Task<ResponseBillingJson> Execute(Guid id)
    {
        var loggedUser = await _loggedUser.Get();

        var billing = await _repository.GetById(loggedUser, id);
        if (billing is null)
        {
            throw new NotFoundException(ResourceErrorMessages.BILLING_NOT_FOUND);
        }

        return _mapper.Map<ResponseBillingJson>(billing);
    }
}
