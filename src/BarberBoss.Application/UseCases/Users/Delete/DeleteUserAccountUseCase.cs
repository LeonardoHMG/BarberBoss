using BarberBoss.Domain.Repositories;
using BarberBoss.Domain.Repositories.Billings;
using BarberBoss.Domain.Repositories.User;
using BarberBoss.Domain.Services.LoggedUser;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;

namespace BarberBoss.Application.UseCases.Users.Delete;

public class DeleteUserAccountUseCase : IDeleteUserAccountUseCase
{
    private readonly IUserReadOnlyRepository _readRepository;
    private readonly IUserWriteOnlyRepository _writeRepository;
    private readonly IUserUpdateOnlyRepository _updateRepository;
    private readonly IBillingsReadOnlyRepository _billingsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoggedUser _loggedUser;

    public DeleteUserAccountUseCase(
        IUserReadOnlyRepository readRepository,
        IUserWriteOnlyRepository writeRepository,
        IUserUpdateOnlyRepository updateRepository,
        IBillingsReadOnlyRepository billingsRepository,
        IUnitOfWork unitOfWork,
        ILoggedUser loggedUser)
    {
        _readRepository = readRepository;
        _writeRepository = writeRepository;
        _updateRepository = updateRepository;
        _billingsRepository = billingsRepository;
        _unitOfWork = unitOfWork;
        _loggedUser = loggedUser;
    }

    public async Task Execute(Guid id)
    {
        var loggedUser = await _loggedUser.Get();

        if (loggedUser.Id == id)
            throw new ForbiddenException(ResourceErrorMessages.CANNOT_DELETE_OWN_ACCOUNT);

        var user = await _readRepository.GetById(id);

        if (user is null)
            throw new NotFoundException(ResourceErrorMessages.USER_NOT_FOUND);

        var hasBillings = await _billingsRepository.HasAnyBillingForUser(id);

        if (hasBillings)
        {
            var trackedUser = await _updateRepository.GetById(id);
            trackedUser.Deactivate();
            _updateRepository.Update(trackedUser);
        }
        else
        {
            await _writeRepository.Delete(id);
        }

        await _unitOfWork.Commit();
    }
}
