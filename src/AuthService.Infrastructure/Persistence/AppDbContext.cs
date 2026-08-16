using AuthService.Application.Abstractions.Persistence;
using AuthService.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace AuthService.Infrastructure.Persistence
{
    public sealed class AppDbContext(
        DbContextOptions<AppDbContext> options)
        : DbContext(options), IUnitOfWork
    {
        public DbSet<User> Users => Set<User>();

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await base.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException exception)
                when (exception.InnerException is PostgresException
                {
                    SqlState: PostgresErrorCodes.UniqueViolation,
                    ConstraintName: "ux_users_normalized_email"
                })
            {
                throw new PersistenceConflictException(
                    PersistenceConflictCodes.UserEmailAlreadyExists,
                    "A user with this email already exists.",
                    exception);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
