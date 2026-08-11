using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Services.LoggedUser;
using BarberBoss.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BarberBoss.Infrastructure.Services.LoggedUser;
internal class LoggedUser : ILoggedUser
{
    private readonly BarberBossDbContext _dbContext;

    public LoggedUser(BarberBossDbContext dbContext) => _dbContext = dbContext;

    public async Task<User> Get()
    {
        string token = "token";

        var tokenHandler = new JwtSecurityTokenHandler();

        var jwtSecurityToken = tokenHandler.ReadJwtToken(token);

        var identifier = jwtSecurityToken.Claims.First(claim => claim.Type == ClaimTypes.Sid).Value;

        return await _dbContext
            .Users
            .AsNoTracking()
            .FirstAsync(user => user.Id == Guid.Parse(identifier));
    }
}
