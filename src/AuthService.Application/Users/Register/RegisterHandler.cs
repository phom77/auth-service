using AuthService.Application.Abstractions.Authentication;
using AuthService.Application.Abstractions.Persistence;
using AuthService.Domain.Users;

namespace AuthService.Application.Users.Register
{
    public sealed class RegisterHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        TimeProvider timeProvider)
    {
        public async Task<RegisterResult> HandleAsync(
            RegisterCommand command,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(command);

            if (!EmailAddress.TryCreate(
                    command.Email,
                    out var email))
            {
                return RegisterResult.InvalidEmail();
            }

            if (!PasswordPolicy.IsValid(command.Password))
            {
                return RegisterResult.WeakPassword();
            }

            var emailAlreadyExists =
                await userRepository.ExistsByNormalizedEmailAsync(
                    email.NormalizedValue,
                    cancellationToken);

            if (emailAlreadyExists)
            {
                return RegisterResult.EmailAlreadyExists();
            }

            var passwordHash =
                passwordHasher.Hash(command.Password);

            var user = User.Create(
                email,
                passwordHash,
                timeProvider.GetUtcNow());

            await userRepository.AddAsync(
                user,
                cancellationToken);

            try
            {
                await unitOfWork.SaveChangesAsync(
                    cancellationToken);
            }
            catch (PersistenceConflictException exception)
                when (exception.ConflictCode ==
                      PersistenceConflictCodes.UserEmailAlreadyExists)
            {
                return RegisterResult.EmailAlreadyExists();
            }

            return RegisterResult.Succeeded(
                user.Id,
                user.Email);
        }
    }
}
