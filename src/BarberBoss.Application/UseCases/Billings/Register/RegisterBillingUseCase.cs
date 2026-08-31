using AutoMapper;
using BarberBoss.Communication.Requests;
using BarberBoss.Communication.Responses;
using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Enums;
using BarberBoss.Domain.Repositories;
using BarberBoss.Domain.Repositories.Billings;
using BarberBoss.Domain.Services.LoggedUser;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;

namespace BarberBoss.Application.UseCases.Billings.Register;

public class RegisterBillingUseCase : IRegisterBillingUseCase
{
    private readonly IBillingsWriteOnlyRepository _writeOnlyRepository;
    private readonly IBillingsReadOnlyRepository _readOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILoggedUser _loggedUser;

    public RegisterBillingUseCase(
        IBillingsWriteOnlyRepository writeOnlyRepository, 
        IBillingsReadOnlyRepository readOnlyRepository, 
        IUnitOfWork unitOfWork, 
        IMapper mapper, 
        ILoggedUser loggedUser)
    {
        _writeOnlyRepository = writeOnlyRepository;
        _readOnlyRepository = readOnlyRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _loggedUser = loggedUser;
    }

    public async Task<ResponseRegisterBillingJson> Execute(RequestBillingJson request)
    {
        var loggedUser = await _loggedUser.Get();

        if (loggedUser.Role == Roles.ADMIN)
            throw new ForbiddenException(ResourceErrorMessages.ADMIN_CANNOT_REGISTER_BILLING);

        Validate(request);

        var exists = await _readOnlyRepository.Exists(loggedUser.Id, request.ClientName, request.ServiceName, request.ServiceDate);

        if (exists)
            throw new ConflictException(ResourceErrorMessages.BILLING_ALREADY_EXISTS);

        var billing = Billing.Register(
              userId: loggedUser.Id,
              serviceDate: request.ServiceDate,
              clientName: request.ClientName,
              serviceName: request.ServiceName,
              amount: request.Amount,
              paymentMethod: (PaymentMethod)request.PaymentMethod,
              status: (PaymentStatus)request.Status,
              notes: request.Notes
        );

        await _writeOnlyRepository.Add(billing);

        await _unitOfWork.Commit();

        return _mapper.Map<ResponseRegisterBillingJson>(billing);
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
