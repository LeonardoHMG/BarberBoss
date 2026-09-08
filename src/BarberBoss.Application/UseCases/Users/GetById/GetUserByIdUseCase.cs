using AutoMapper;
using BarberBoss.Communication.Responses;
using BarberBoss.Domain.Repositories.User;
using BarberBoss.Domain.Services.LoggedUser;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;

namespace BarberBoss.Application.UseCases.Users.GetById;
public class GetUserByIdUseCase : IGetUserByIdUseCase
{
    private readonly IUserReadOnlyRepository _repository;
    private readonly IMapper _mapper;

    public GetUserByIdUseCase(
        IUserReadOnlyRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ResponseUserJson> Execute(Guid id)
    { 
        var user = await _repository.GetById(id);

        if (user is null)
        {
            throw new NotFoundException(ResourceErrorMessages.USER_NOT_FOUND);
        }

        return _mapper.Map<ResponseUserJson>(user);
    }
}
