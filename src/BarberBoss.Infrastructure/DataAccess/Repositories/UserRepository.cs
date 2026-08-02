using BarberBoss.Domain.Repositories.User;
using Microsoft.EntityFrameworkCore;

namespace BarberBoss.Infrastructure.DataAccess.Repositories;
internal class UserRepository : IUserReadOnlyRepository
{
    private readonly BarberBossDbContext _dbContext;

    public UserRepository(BarberBossDbContext dbContext) => _dbContext = dbContext;

    public async Task<bool> ExistActiveUserWithEmail(string email)
    {
        return await _dbContext.Users.AnyAsync(user => user.Email.Equals(email));
    }
}
