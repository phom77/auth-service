using AuthService.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Application.Abstractions.Persistence
{
    public interface IUserRepository
    {
        Task<bool> ExistsByNormalizedEmailAsync(
            string normalizedEmail,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            User user,
            CancellationToken cancellationToken= default);
    }
}
