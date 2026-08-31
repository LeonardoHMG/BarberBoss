using AutoMapper;
using BarberBoss.Communication.Requests;
using BarberBoss.Domain.Enums;
using BarberBoss.Domain.Repositories;
using BarberBoss.Domain.Repositories.Billings;
using BarberBoss.Domain.Services.LoggedUser;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;

namespace BarberBoss.Application.UseCases.Billings.Update;

public class UpdateBillingUseCase : IUpdateBillingUseCase
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBillingsUpdateOnlyRepository _repository;
    private readonly ILoggedUser _loggedUser;

    public UpdateBillingUseCase(
        IMapper mapper, 
        IUnitOfWork unitOfWork, 
        IBillingsUpdateOnlyRepository repository,
        ILoggedUser loggedUser)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _repository = repository;
        _loggedUser = loggedUser;
    }

    public async Task Execute(Guid id, RequestBillingJson request)
    {
        Validate(request);

        var loggedUser = await _loggedUser.Get();

        var billing = await _repository.GetById(loggedUser, id);

        if (billing is null)
        {
            throw new NotFoundException(ResourceErrorMessages.BILLING_NOT_FOUND);
        }

        billing.UpdateDetails(
              serviceDate: request.ServiceDate,
              clientName: request.ClientName,
              serviceName: request.ServiceName,
              amount: request.Amount,
              paymentMethod: (PaymentMethod)request.PaymentMethod,
              status: (PaymentStatus)request.Status,
              notes: request.Notes
         );

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
