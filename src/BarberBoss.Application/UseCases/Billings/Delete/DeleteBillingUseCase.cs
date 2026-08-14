using BarberBoss.Domain.Repositories;
using BarberBoss.Domain.Repositories.Billings;
using BarberBoss.Domain.Services.LoggedUser;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;

namespace BarberBoss.Application.UseCases.Billings.Delete;

public class DeleteBillingUseCase : IDeleteBillingUseCase
{
    private readonly IBillingsReadOnlyRepository _readRepository;
    private readonly IBillingsWriteOnlyRepository _writeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoggedUser _loggedUser;

    public DeleteBillingUseCase(
        IBillingsWriteOnlyRepository writeRepositor, 
        IUnitOfWork unitOfWork, 
        ILoggedUser user,
        IBillingsReadOnlyRepository readRepository)
    {
        _writeRepository = writeRepositor;
        _unitOfWork = unitOfWork;
        _loggedUser = user;
        _readRepository = readRepository;
    }

    public async Task Execute(Guid id)
    {
        var loggedUser = await _loggedUser.Get();

        var biiling = await _readRepository.GetById(loggedUser, id);
        if (biiling is null)
        {
            throw new NotFoundException(ResourceErrorMessages.BILLING_NOT_FOUND);
        }

        await _writeRepository.Delete(id);

        await _unitOfWork.Commit();
    }
}
