using AutoMapper;
using BarberBoss.Communication.Requests;
using BarberBoss.Communication.Responses;
using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Repositories;
using BarberBoss.Domain.Repositories.Billings;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;

namespace BarberBoss.Application.UseCases.Billings.Register;

public class RegisterBillingUseCase : IRegisterBillingUseCase
{
    private readonly IBillingsWriteOnlyRepository _writeOnlyRepository;
    private readonly IBillingsReadOnlyRepository _readOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RegisterBillingUseCase(IBillingsWriteOnlyRepository writeOnlyRepository, IBillingsReadOnlyRepository readOnlyRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _writeOnlyRepository = writeOnlyRepository;
        _readOnlyRepository = readOnlyRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ResponseRegisterBillingJson> Execute(RequestBillingJson request)
    {
        Validate(request);

        var exists = await _readOnlyRepository.Exists(request.BarberName, request.ClientName, request.ServiceName, request.Date);

        if (exists)
            throw new ConflictException(ResourceErrorMessages.BILLING_EXISTS);

        var entity = _mapper.Map<Billing>(request);

        await _writeOnlyRepository.Add(entity);

        await _unitOfWork.Commit();

        return _mapper.Map<ResponseRegisterBillingJson>(entity);
    }

    private void Validate(RequestBillingJson request)
    {
        var validator = new BillingValidator();

        var result = validator.Validate(request);   

        if (result.IsValid ==  false)
        {
            var errorMessages = result.Errors.Select(f => f.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
