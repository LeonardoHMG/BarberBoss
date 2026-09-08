using AutoMapper;
using BarberBoss.Communication.Requests;
using BarberBoss.Domain.Repositories;
using BarberBoss.Domain.Repositories.User;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;

namespace BarberBoss.Application.UseCases.Users.UpdateById;

public class UpdateUserByIdUseCase : IUpdateUserByIdUseCase
{
    private readonly IUserUpdateOnlyRepository _repository;
    private readonly IUserReadOnlyRepository _readOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserByIdUseCase(
        IUserUpdateOnlyRepository repository,
        IUserReadOnlyRepository readOnlyRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _readOnlyRepository = readOnlyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(Guid id, RequestUpdateUserByAdminJson request)
    {
        Validate(request);

        var user = await _repository.GetById(id);

        if (user is null)
        {
            throw new NotFoundException(ResourceErrorMessages.USER_NOT_FOUND);
        }

        if (user.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase) == false)
        {
            var userWithSameEmail = await _readOnlyRepository.GetUserByEmail(request.Email);

            if (userWithSameEmail is not null && userWithSameEmail.Id != id)
            {
                throw new ConflictException(ResourceErrorMessages.EMAIL_ALREADY_REGISTERED);
            }
        }

        user.UpdateByAdmin(request.Name, request.Email, request.Role, request.IsActive);

        _repository.Update(user);

        await _unitOfWork.Commit();
    }

    private void Validate(RequestUpdateUserByAdminJson request)
    {
        var validator = new UpdateUserByAdminValidator();
        var result = validator.Validate(request);

        if (result.IsValid == false)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
