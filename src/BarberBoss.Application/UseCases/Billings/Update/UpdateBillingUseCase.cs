using AutoMapper;
using BarberBoss.Communication.Requests;
using BarberBoss.Domain.Repositories;
using BarberBoss.Domain.Repositories.Billings;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;

namespace BarberBoss.Application.UseCases.Billings.Update;

public class UpdateBillingUseCase : IUpdateBillingUseCase
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBillingUpdateOnlyRepository _repository;

    public UpdateBillingUseCase(IMapper mapper, IUnitOfWork unitOfWork, IBillingUpdateOnlyRepository repository)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task Execute(Guid id, RequestBillingJson request)
    {
        Validate(request);

        var billing = await _repository.GetById(id);

        if (billing is null)
        {
            throw new NotFoundException("Faturamento não encontrado.");
        }

        _mapper.Map(request, billing);

        _repository.Update(billing);

        await _unitOfWork.Commit();
    }

    private void Validate(RequestBillingJson request)
    {
        var validator = new BillingValidator();

        var result = validator.Validate(request);

        if (result.IsValid == false)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
