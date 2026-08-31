using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Repositories.User;
using Microsoft.EntityFrameworkCore;

namespace BarberBoss.Infrastructure.DataAccess.Repositories;
internal class UserRepository : IUserReadOnlyRepository, IUserWriteOnlyRepository, IUserUpdateOnlyRepository
{
    private readonly BarberBossDbContext _dbContext;

    public UserRepository(BarberBossDbContext dbContext) => _dbContext = dbContext;

    public async Task Add(User user)
    {
        await _dbContext.Users.AddAsync(user);
    }

    public async Task<bool> ExistActiveUserWithEmail(string email)
    {
        return await _dbContext.Users.AnyAsync(u => u.Email == email && u.IsActive);
    }

    async Task<User> IUserUpdateOnlyRepository.GetById(Guid Id)
    {
        return await _dbContext.Users.FirstAsync(user => user.Id == Id);
    }

    async Task<User?> IUserReadOnlyRepository.GetById(Guid id)
    {
        return await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(user => user.Email.Equals(email));
    }

    public void Update(User user)
    {
        _dbContext.Users.Update(user);
    }

    public async Task Delete(Guid id)
    {
        var user = await _dbContext.Users.FirstAsync(u => u.Id == id);

        _dbContext.Users.Remove(user);
    }
}
