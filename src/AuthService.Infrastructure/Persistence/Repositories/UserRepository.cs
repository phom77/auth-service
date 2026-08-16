using AuthService.Application.Abstractions.Persistence;
using AuthService.Domain.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Infrastructure.Persistence.Repositories
{
    internal sealed class UserRepository(AppDbContext dbContext) : IUserRepository
    {
        public async Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            await dbContext.Users.AddAsync(user, cancellationToken);
        }

        public Task<bool> ExistsByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
        {
            return dbContext.Users.AnyAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);
        }
    }
}
