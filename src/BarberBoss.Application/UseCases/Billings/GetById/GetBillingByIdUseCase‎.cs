using AutoMapper;
using BarberBoss.Communication.Responses;
using BarberBoss.Domain.Repositories.Billings;

namespace BarberBoss.Application.UseCases.Billings.GetById;

public class GetBillingByIdUseCase‎ : IGetBillingByIdUseCase
{
    private readonly IBillingsRepository _repository;
    private readonly IMapper _mapper;

    public GetBillingByIdUseCase‎(IBillingsRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ResponseBillingJson> Execute(Guid id)
    {
        var result = await _repository.GetById(id);

        return _mapper.Map<ResponseBillingJson>(result);
    }
}
