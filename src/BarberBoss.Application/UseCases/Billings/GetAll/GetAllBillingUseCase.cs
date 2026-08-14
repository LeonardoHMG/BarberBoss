using AutoMapper;
using BarberBoss.Communication.Requests;
using BarberBoss.Communication.Responses;
using BarberBoss.Domain.Repositories.Billings;
using BarberBoss.Domain.Services.LoggedUser;
using BarberBoss.Exception.ExceptionsBase;
using FluentValidation;

namespace BarberBoss.Application.UseCases.Billings.GetAll;

public class GetAllBillingUseCase : IGetAllBillingUseCase
{
    private readonly IBillingsReadOnlyRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILoggedUser _loggedUser;

    public GetAllBillingUseCase(
        IBillingsReadOnlyRepository repository, 
        IMapper mapper,
        ILoggedUser loggedUser)
    {
        _repository = repository;
        _mapper = mapper;
        _loggedUser = loggedUser;
    }

    public async Task<ResponseBillingsJson> Execute(RequestGetBillingsJson request)
    {
        Validate(request);

        var filter = _mapper.Map<BillingFilter>(request);

        var loggedUser = await _loggedUser.Get();

        var result = await _repository.GetAll(loggedUser, filter);

        var response = new ResponseBillingsJson
        {
            Billings = _mapper.Map<List<ResponseShortBillingJson>>(result.Items),
            TotalCount = result.TotalCount,
            CurrentPage = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = request.PageSize > 0
                ? (int)Math.Ceiling((double)result.TotalCount / request.PageSize)
                : 0
        };

        return response;
    }

    private void Validate(RequestGetBillingsJson request)
    {
        var validator = new GetAllBillingValidator();

        var result = validator.Validate(request);

        if (result.IsValid == false)
        {
            var errorMessages = result.Errors.Select(f => f.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
