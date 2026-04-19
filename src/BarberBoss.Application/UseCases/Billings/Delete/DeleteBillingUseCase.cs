using BarberBoss.Domain.Repositories;
using BarberBoss.Domain.Repositories.Billings;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;

namespace BarberBoss.Application.UseCases.Billings.Delete;

public class DeleteBillingUseCase : IDeleteBillingUseCase
{
    private readonly IBillingsWriteOnlyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBillingUseCase(IBillingsWriteOnlyRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(Guid id)
    {
        var result = await _repository.Delete(id);

        if (result == false)
        {
            throw new NotFoundException(ResourceErrorMessages.BILLING_NOT_FOUND);
        }

        await _unitOfWork.Commit();
    }
}
