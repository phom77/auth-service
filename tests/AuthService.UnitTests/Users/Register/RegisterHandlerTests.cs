using AuthService.Application.Abstractions.Authentication;
using AuthService.Application.Abstractions.Persistence;
using AuthService.Application.Users.Register;
using AuthService.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.UnitTests.Users.Register
{
    public sealed class RegisterHandlerTests
    {
        private static readonly DateTimeOffset FixedUtcNow =
            new(2026, 8, 13, 0, 0, 0, TimeSpan.Zero);

        [Fact]
        public async Task HandleAsync_WithValidInput_CreatesUser()
        {
            var repository = new FakeUserRepository();
            var unitOfWork = new FakeUnitOfWork();
            var passwordHasher = new FakePasswordHasher();

            var handler = CreateHandler(
                repository,
                unitOfWork,
                passwordHasher);

            var result = await handler.HandleAsync(
                new RegisterCommand(
                    " Student@Example.com ",
                    "a-valid-password"));

            Assert.Equal(RegisterStatus.Success, result.Status);
            Assert.NotNull(repository.AddedUser);

            Assert.Equal(
                "Student@Example.com",
                repository.AddedUser.Email);

            Assert.Equal(
                "STUDENT@EXAMPLE.COM",
                repository.AddedUser.NormalizedEmail);

            Assert.Equal(
                "hashed:a-valid-password",
                repository.AddedUser.PasswordHash);

            Assert.NotEqual(
                "a-valid-password",
                repository.AddedUser.PasswordHash);

            Assert.Equal(FixedUtcNow, repository.AddedUser.CreatedAtUtc);
            Assert.Equal(1, unitOfWork.SaveCallCount);
        }

        [Fact]
        public async Task HandleAsync_WhenEmailExists_DoesNotCreateUser()
        {
            var repository = new FakeUserRepository
            {
                EmailExists = true
            };

            var unitOfWork = new FakeUnitOfWork();
            var passwordHasher = new FakePasswordHasher();

            var handler = CreateHandler(
                repository,
                unitOfWork,
                passwordHasher);

            var result = await handler.HandleAsync(
                new RegisterCommand(
                    "student@example.com",
                    "a-valid-password"));

            Assert.Equal(
                RegisterStatus.EmailAlreadyExists,
                result.Status);

            Assert.Null(repository.AddedUser);
            Assert.Equal(0, passwordHasher.HashCallCount);
            Assert.Equal(0, unitOfWork.SaveCallCount);
        }

        [Fact]
        public async Task HandleAsync_WhenDatabaseDetectsRace_ReturnsConflict()
        {
            var repository = new FakeUserRepository();

            var unitOfWork = new FakeUnitOfWork
            {
                ExceptionToThrow =
                    new PersistenceConflictException(
                        PersistenceConflictCodes.UserEmailAlreadyExists,
                        "Email already exists.")
            };

            var handler = CreateHandler(
                repository,
                unitOfWork,
                new FakePasswordHasher());

            var result = await handler.HandleAsync(
                new RegisterCommand(
                    "student@example.com",
                    "a-valid-password"));

            Assert.Equal(
                RegisterStatus.EmailAlreadyExists,
                result.Status);
        }

        [Theory]
        [InlineData("")]
        [InlineData("short")]
        [InlineData("12345678901")]
        public async Task HandleAsync_WithWeakPassword_ReturnsValidationFailure(
            string password)
        {
            var repository = new FakeUserRepository();
            var unitOfWork = new FakeUnitOfWork();

            var handler = CreateHandler(
                repository,
                unitOfWork,
                new FakePasswordHasher());

            var result = await handler.HandleAsync(
                new RegisterCommand(
                    "student@example.com",
                    password));

            Assert.Equal(
                RegisterStatus.WeakPassword,
                result.Status);

            Assert.Null(repository.AddedUser);
            Assert.Equal(0, unitOfWork.SaveCallCount);
        }

        private static RegisterHandler CreateHandler(
            FakeUserRepository repository,
            FakeUnitOfWork unitOfWork,
            FakePasswordHasher passwordHasher)
        {
            return new RegisterHandler(
                repository,
                unitOfWork,
                passwordHasher,
                new StubTimeProvider(FixedUtcNow));
        }

        private sealed class FakeUserRepository
            : IUserRepository
        {
            public bool EmailExists { get; init; }

            public User? AddedUser { get; private set; }

            public Task<bool> ExistsByNormalizedEmailAsync(
                string normalizedEmail,
                CancellationToken cancellationToken = default)
            {
                return Task.FromResult(EmailExists);
            }

            public Task AddAsync(
                User user,
                CancellationToken cancellationToken = default)
            {
                AddedUser = user;
                return Task.CompletedTask;
            }
        }

        private sealed class FakeUnitOfWork
            : IUnitOfWork
        {
            public int SaveCallCount { get; private set; }

            public Exception? ExceptionToThrow { get; init; }

            public Task<int> SaveChangesAsync(
                CancellationToken cancellationToken = default)
            {
                SaveCallCount++;

                if (ExceptionToThrow is not null)
                {
                    throw ExceptionToThrow;
                }

                return Task.FromResult(1);
            }
        }

        private sealed class FakePasswordHasher
            : IPasswordHasher
        {
            public int HashCallCount { get; private set; }

            public string Hash(string password)
            {
                HashCallCount++;
                return $"hashed:{password}";
            }

            public PasswordVerificationResult Verify(
                string passwordHash,
                string providedPassword)
            {
                return PasswordVerificationResult.Failed;
            }
        }

        private sealed class StubTimeProvider(
            DateTimeOffset utcNow)
            : TimeProvider
        {
            public override DateTimeOffset GetUtcNow()
            {
                return utcNow;
            }
        }
    }
}
